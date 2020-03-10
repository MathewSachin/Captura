using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Captura;
using Captura.Audio;

// ReSharper disable MethodSupportsCancellation

namespace Screna
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
        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="IRecorder"/> writing to <see cref="IVideoFileWriter"/>.
        /// </summary>
        /// <param name="VideoWriter">The <see cref="IVideoFileWriter"/> to write to.</param>
        /// <param name="ImageProvider">The image source.</param>
        /// <param name="FrameRate">Video Frame Rate.</param>
        /// <param name="AudioProvider">The audio source. null = no audio.</param>
        public Recorder(IVideoFileWriter VideoWriter, IImageProvider ImageProvider, int FrameRate, IAudioProvider AudioProvider = null)
        {
            _videoWriter = VideoWriter ?? throw new ArgumentNullException(nameof(VideoWriter));
            _imageProvider = ImageProvider ?? throw new ArgumentNullException(nameof(ImageProvider));
            _audioProvider = AudioProvider;

            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;

            if (FrameRate <= 0)
                throw new ArgumentException("Frame Rate must be possitive", nameof(FrameRate));

            _frameRate = FrameRate;

            _continueCapturing = new ManualResetEvent(false);

            if (VideoWriter.SupportsAudio && AudioProvider != null)
            {
                var wf = AudioProvider.WaveFormat;

                _audioBytesPerFrame = (int)((1.0 / FrameRate)
                                              * wf.SampleRate
                                              * wf.Channels
                                              * (wf.BitsPerSample / 8.0));
            }
            else _audioProvider = null;

            _sw = new Stopwatch();

            _recordTask = Task.Factory.StartNew(async () => await DoRecord(), TaskCreationOptions.LongRunning);
        }

        Task<bool> _task;
        int _frameCount;
        int _audioBytesWritten;
        readonly int _audioBytesPerFrame;
        byte[] _audioBuffer, _silenceBuffer;
        bool _alternateWriteAudio = true;

        async Task DoRecord()
        {
            try
            {
                var frameInterval = TimeSpan.FromSeconds(1.0 / _frameRate);
                _frameCount = 0;

                // Returns false when stopped
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

                bool CanContinue()
                {
                    try
                    {
                        return _continueCapturing.WaitOne();
                    }
                    catch (ObjectDisposedException)
                    {
                        return false;
                    }
                }

                while (CanContinue() && !_cancellationToken.IsCancellationRequested)
                {
                    var timestamp = _sw.Elapsed;

                    if (_task != null)
                    {
                        // If false, stop recording
                        if (!await _task)
                            return;

                        var requiredFrames = _sw.Elapsed.TotalSeconds * _frameRate;
                        var diff = requiredFrames - _frameCount;

                        // Write atmost 1 duplicate frame
                        if (diff >= 1)
                        {
                            if (!AddFrame(RepeatFrame.Instance))
                                return;
                        }
                    }

                    _task = Task.Factory.StartNew(() =>
                    {
                        var editableFrame = _imageProvider.Capture();

                        if (_cancellationToken.IsCancellationRequested)
                            return false;

                        var frame = editableFrame.GenerateFrame(timestamp);

                        if (_cancellationToken.IsCancellationRequested)
                            return false;

                        var success = AddFrame(frame);

                        if (!success)
                        {
                            return false;
                        }

                        try
                        {
                            WriteAudio();
                            return true;
                        }
                        catch (InvalidOperationException)
                        {
                            return false;
                        }
                    });

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

        void WriteAudio()
        {
            if (_audioProvider == null)
            {
                return;
            }

            _alternateWriteAudio = !_alternateWriteAudio;

            if (!_alternateWriteAudio)
            {
                return;
            }

            var shouldHaveWritten = (_frameCount - 1) * _audioBytesPerFrame;

            if (_audioBytesWritten >= shouldHaveWritten)
            {
                return;
            }

            var toWrite = shouldHaveWritten - _audioBytesWritten;

            if (_audioBuffer == null || _audioBuffer.Length < toWrite)
            {
                _audioBuffer = new byte[toWrite];
            }

            var read = _audioProvider.Read(_audioBuffer, 0, toWrite);

            _videoWriter.WriteAudio(_audioBuffer, 0, read);
            _audioBytesWritten += read;

            var silenceToWrite = toWrite - read;

            if (silenceToWrite <= 0)
            {
                return;
            }
            
            if (_silenceBuffer == null || _silenceBuffer.Length < silenceToWrite)
            {
                _silenceBuffer = new byte[silenceToWrite];
            }

            _videoWriter.WriteAudio(_silenceBuffer, 0, silenceToWrite);
            _audioBytesWritten += silenceToWrite;
        }

        #region Dispose
        void Dispose(bool TerminateRecord)
        {
            if (_disposed)
                return;

            _disposed = true;

            if (_audioProvider != null)
            {
                _audioProvider.Stop();
                _audioProvider.Dispose();
                _audioProvider = null;
            }

            _cancellationTokenSource.Cancel();

            // Resume record loop if paused so it can exit
            _continueCapturing.Set();

            if (TerminateRecord)
                _recordTask.Wait();

            try
            {
                if (_task != null && !_task.IsCompleted)
                    _task.GetAwaiter().GetResult();
            }
            catch { }

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
