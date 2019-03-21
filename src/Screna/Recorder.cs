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
        IAudioFileWriter _audioWriter;
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
                AudioProvider.DataAvailable += AudioProvider_DataAvailable;
            else _audioProvider = null;

            _sw = new Stopwatch();

            _recordTask = Task.Factory.StartNew(async () => await DoRecord(), TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Creates a new instance of <see cref="IRecorder"/> writing to <see cref="IAudioFileWriter"/>.
        /// </summary>
        /// <param name="AudioWriter">The <see cref="IAudioFileWriter"/> to write to.</param>
        /// <param name="AudioProvider">The audio source.</param>
        public Recorder(IAudioFileWriter AudioWriter, IAudioProvider AudioProvider)
        {
            _audioWriter = AudioWriter ?? throw new ArgumentNullException(nameof(AudioWriter));
            _audioProvider = AudioProvider ?? throw new ArgumentNullException(nameof(AudioProvider));

            _audioProvider.DataAvailable += AudioProvider_DataAvailable;
        }

        Task<bool> _task;

        async Task DoRecord()
        {
            try
            {
                var frameInterval = TimeSpan.FromSeconds(1.0 / _frameRate);
                var frameCount = 0;

                // Returns false when stopped
                bool AddFrame(IBitmapFrame Frame)
                {
                    try
                    {
                        _videoWriter.WriteFrame(Frame);

                        ++frameCount;

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
                    var timestamp = DateTime.Now;

                    if (_task != null)
                    {
                        // If false, stop recording
                        if (!await _task)
                            return;

                        var requiredFrames = _sw.Elapsed.TotalSeconds * _frameRate;
                        var diff = requiredFrames - frameCount;

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

                        var frame = editableFrame.GenerateFrame();

                        if (_cancellationToken.IsCancellationRequested)
                            return false;

                        return AddFrame(frame);
                    });

                    var timeTillNextFrame = timestamp + frameInterval - DateTime.Now;

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

        void AudioProvider_DataAvailable(object Sender, DataAvailableEventArgs E)
        {
            if (_videoWriter == null)
            {
                _audioWriter.Write(E.Buffer, 0, E.Length);
                return;
            }

            try
            {
                lock (_syncLock)
                {
                    if (_disposed)
                        return;
                }

                _videoWriter.WriteAudio(E.Buffer, E.Length);
            }
            catch (Exception e)
            {
                if (_imageProvider == null)
                {
                    lock (_syncLock)
                    {
                        if (!_disposed)
                        {
                            ErrorOccurred?.Invoke(e);

                            Dispose(true);
                        }
                    }
                }
            }
        }

        #region Dispose
        void Dispose(bool TerminateRecord)
        {
            if (_disposed)
                return;

            _disposed = true;

            if (_audioProvider != null)
            {
                _audioProvider.DataAvailable -= AudioProvider_DataAvailable;
                _audioProvider.Stop();
                _audioProvider.Dispose();
                _audioProvider = null;
            }

            if (_videoWriter != null)
            {
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

                _videoWriter.Dispose();
                _videoWriter = null;

                _continueCapturing.Dispose();
            }
            else
            {
                _audioWriter.Dispose();
                _audioWriter = null;
            }

            _imageProvider?.Dispose();
            _imageProvider = null;
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
