using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Captura.Audio;
using Captura.Models;

// ReSharper disable MethodSupportsCancellation

namespace Captura.Video
{
    /// <summary>
    /// Default implementation of <see cref="IRecorder"/> interface.
    /// Can output to <see cref="IVideoFileWriter"/> or <see cref="IAudioFileWriter"/>.
    /// </summary>
    public class Recorder : IRecorder
    {
        #region Fields
        IAudioProvider _audioProvider;
        IVideoFileWriter _videoWriter;
        IImageProvider _imageProvider;

        readonly int _frameRate;

        readonly Stopwatch _sw;

        readonly ManualResetEvent _continueCapturing;
        readonly CancellationTokenSource _cancellationTokenSource;
        readonly CancellationToken _cancellationToken;

        readonly Task _recordTask;

        readonly object _syncLock = new object();

        Task<bool> _frameWriteTask;
        Task _audioWriteTask;
        int _frameCount;
        long _audioBytesWritten;
        readonly int _audioBytesPerFrame, _audioChunkBytes;
        const int AudioChunkLengthMs = 200;
        byte[] _audioBuffer, _silenceBuffer;

        readonly IFpsManager _fpsManager;
        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="IRecorder"/> writing to <see cref="IVideoFileWriter"/>.
        /// </summary>
        /// <param name="VideoWriter">The <see cref="IVideoFileWriter"/> to write to.</param>
        /// <param name="ImageProvider">The image source.</param>
        /// <param name="FrameRate">Video Frame Rate.</param>
        /// <param name="AudioProvider">The audio source. null = no audio.</param>
        public Recorder(IVideoFileWriter VideoWriter, IImageProvider ImageProvider, int FrameRate,
            IAudioProvider AudioProvider = null,
            IFpsManager FpsManager = null)
        {
            _videoWriter = VideoWriter ?? throw new ArgumentNullException(nameof(VideoWriter));
            _imageProvider = ImageProvider ?? throw new ArgumentNullException(nameof(ImageProvider));
            _audioProvider = AudioProvider;
            _fpsManager = FpsManager;

            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;

            if (FrameRate <= 0)
                throw new ArgumentException("Frame Rate must be possitive", nameof(FrameRate));

            _frameRate = FrameRate;

            _continueCapturing = new ManualResetEvent(false);

            if (VideoWriter.SupportsAudio && AudioProvider != null)
            {
                var wf = AudioProvider.WaveFormat;

                _audioBytesPerFrame = (int) ((1.0 / FrameRate)
                                             * wf.SampleRate
                                             * wf.Channels
                                             * (wf.BitsPerSample / 8.0));

                _audioChunkBytes = (int) (_audioBytesPerFrame * (FrameRate * AudioChunkLengthMs / 1000.0));
            }
            else _audioProvider = null;

            _sw = new Stopwatch();

            _recordTask = Task.Factory.StartNew(async () => await DoRecord(), TaskCreationOptions.LongRunning);
        }

        async Task DoRecord()
        {
            try
            {
                var frameInterval = TimeSpan.FromSeconds(1.0 / _frameRate);
                _frameCount = 0;

                // Returns false when stopped

                while (_continueCapturing.WaitOne() && !_cancellationToken.IsCancellationRequested)
                {
                    var timestamp = _sw.Elapsed;

                    if (_frameWriteTask != null)
                    {
                        // If false, stop recording
                        if (!await _frameWriteTask)
                            return;

                        if (!WriteDuplicateFrame())
                            return;
                    }

                    if (_audioWriteTask != null)
                    {
                        await _audioWriteTask;
                    }

                    _frameWriteTask = Task.Run(() => FrameWriter(timestamp));

                    var timeTillNextFrame = timestamp + frameInterval - _sw.Elapsed;

                    if (timeTillNextFrame > TimeSpan.Zero)
                        Thread.Sleep(timeTillNextFrame);
                }
            }
            catch (Exception e)
            {
                lock (_syncLock)
                {
                    if (!_disposed)
                    {
                        ErrorOccurred?.Invoke(e);

                        Dispose(false);
                    }
                }
            }
        }

        bool FrameWriter(TimeSpan Timestamp)
        {
            var editableFrame = _imageProvider.Capture();

            var frame = editableFrame.GenerateFrame(Timestamp);

            var success = AddFrame(frame);

            if (!success)
            {
                return false;
            }

            _fpsManager?.OnFrame();

            try
            {
                _audioWriteTask = Task.Run(WriteAudio);

                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        bool WriteDuplicateFrame()
        {
            var requiredFrames = _sw.Elapsed.TotalSeconds * _frameRate;
            var diff = requiredFrames - _frameCount;

            // Write atmost 1 duplicate frame
            if (diff >= 1)
            {
                if (!AddFrame(RepeatFrame.Instance))
                    return false;
            }

            return true;
        }

        bool AddFrame(IBitmapFrame Frame)
        {
            try
            {
                _videoWriter.WriteFrame(Frame);

                ++_frameCount;

                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        void WriteAudio()
        {
            if (_audioProvider == null)
            {
                return;
            }

            // These values need to be long otherwise can get out of range in a few hours
            var shouldHaveWritten = (_frameCount - 1L) * _audioBytesPerFrame;

            // Already written more than enough, skip for now
            if (_audioBytesWritten >= shouldHaveWritten)
            {
                return;
            }

            var toWrite = (int)(shouldHaveWritten - _audioBytesWritten);

            // Only write if data to write is more than chunk size.
            // This gives enough time for the audio provider to buffer data from the source.
            if (toWrite < _audioChunkBytes)
            {
                return;
            }

            // Reallocate buffer as needed
            if (_audioBuffer == null || _audioBuffer.Length < toWrite)
            {
                _audioBuffer = new byte[toWrite];
            }

            var read = _audioProvider.Read(_audioBuffer, 0, toWrite);

            // Nothing read
            if (read == 0)
            {
                return;
            }

            _videoWriter.WriteAudio(_audioBuffer, 0, read);
            _audioBytesWritten += read;

            // Fill with silence to maintain synchronization
            var silenceToWrite = toWrite - read;

            // Write silence only when more than a threshold
            // Threshold should ideally be a bit greater than chunk size
            if (silenceToWrite <= _audioChunkBytes * 1.5)
            {
                return;
            }
            
            // Reallocate silence buffer: An array of zeros.
            if (_silenceBuffer == null || _silenceBuffer.Length < silenceToWrite)
            {
                _silenceBuffer = new byte[silenceToWrite];
            }

            _videoWriter.WriteAudio(_silenceBuffer, 0, silenceToWrite);
            _audioBytesWritten += silenceToWrite;
        }

        #region Dispose
        async void Dispose(bool TerminateRecord)
        {
            if (_disposed)
                return;

            _disposed = true;

            _cancellationTokenSource.Cancel();

            // Resume record loop if paused so it can exit
            _continueCapturing.Set();

            // Ensure all threads exit before disposing resources.
            if (TerminateRecord)
                _recordTask.Wait();

            try
            {
                if (_frameWriteTask != null)
                    await _frameWriteTask;
            }
            catch { }

            try
            {
                if (_audioWriteTask != null)
                    await _audioWriteTask;
            }
            catch { }

            if (_audioProvider != null)
            {
                _audioProvider.Stop();
                _audioProvider.Dispose();
                _audioProvider = null;
            }

            _imageProvider?.Dispose();
            _imageProvider = null;

            _videoWriter.Dispose();
            _videoWriter = null;

            _audioBuffer = _silenceBuffer = null;

            _continueCapturing.Dispose();
        }

        /// <summary>
        /// Frees all resources used by this instance.
        /// </summary>
        public void Dispose()
        {
            lock (_syncLock)
            {
                Dispose(true);
            }
        }

        bool _disposed;

        /// <summary>
        /// Fired when an error occurs
        /// </summary>
        public event Action<Exception> ErrorOccurred;

        void ThrowIfDisposed()
        {
            lock (_syncLock)
            {
                if (_disposed)
                    throw new ObjectDisposedException("this");
            }
        }
        #endregion

        /// <summary>
        /// Start Recording.
        /// </summary>
        public void Start()
        {
            ThrowIfDisposed();

            _sw?.Start();

            _audioProvider?.Start();
            
            _continueCapturing?.Set();
        }

        /// <summary>
        /// Stop Recording.
        /// </summary>
        public void Stop()
        {
            ThrowIfDisposed();

            _continueCapturing?.Reset();
            _audioProvider?.Stop();

            _sw?.Stop();
        }
    }
}
