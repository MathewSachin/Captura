using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Captura;
using Captura.Audio;

namespace Screna
{
    /// <summary>
    /// Default implementation of <see cref="IRecorder"/> interface.
    /// Can output to <see cref="IVideoFileWriter"/> or <see cref="IAudioFileWriter"/>.
    /// </summary>
    public class Recorder : IRecorder
    {
        #region Fields
        readonly IAudioProvider _audioProvider;
        readonly IVideoFileWriter _videoWriter;
        readonly IAudioFileWriter _audioWriter;
        readonly IImageProvider _imageProvider;

        readonly int _frameRate, _maxFrameCount, _congestionFrameCount;
        bool _congestion;

        readonly BlockingCollection<IBitmapFrame> _frames;
        readonly Stopwatch _sw;

        readonly ManualResetEvent _continueCapturing;

        readonly Task _writeTask, _recordTask;

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

            if (FrameRate <= 0)
                throw new ArgumentException("Frame Rate must be possitive", nameof(FrameRate));

            _frameRate = FrameRate;
            _congestionFrameCount = _frameRate * 2; // 2 seconds
            _maxFrameCount = _frameRate * 4; // 4 seconds

            _continueCapturing = new ManualResetEvent(false);

            if (VideoWriter.SupportsAudio && AudioProvider != null)
                AudioProvider.DataAvailable += AudioProvider_DataAvailable;
            else _audioProvider = null;

            _sw = new Stopwatch();
            _frames = new BlockingCollection<IBitmapFrame>();

            _recordTask = Task.Factory.StartNew(async () => await DoRecord(), TaskCreationOptions.LongRunning);
            _writeTask = Task.Factory.StartNew(DoWrite, TaskCreationOptions.LongRunning);
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

            _audioProvider.DataAvailable += (S, E) => _audioWriter.Write(E.Buffer, 0, E.Length);
        }

        void DoWrite()
        {
            try
            {
                while (!_frames.IsCompleted)
                {
                    _frames.TryTake(out var img, -1);

                    if (img != null)
                    {
                        // Avoid writing Repeat frames during congestion
                        if (img is RepeatFrame && _congestion)
                        {
                            continue;
                        }

                        _videoWriter.WriteFrame(img);
                    }

                    // Stop Writing
                    if (_frames.Count > _maxFrameCount)
                        break;
                }
            }
            catch (Exception e)
            {
                lock (_syncLock)
                {
                    if (!_disposed)
                    {
                        ErrorOccurred?.Invoke(e);

                        Dispose(true, false);
                    }
                }
            }
        }

        async Task DoRecord()
        {
            try
            {
                var frameInterval = TimeSpan.FromSeconds(1.0 / _frameRate);
                var frameCount = 0;

                Task<bool> task = null;

                // Returns false when stopped
                bool AddFrame(IBitmapFrame Frame)
                {
                    try
                    {
                        _frames.Add(Frame);

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

                while (CanContinue() && !_frames.IsAddingCompleted)
                {
                    if (!_congestion && _frames.Count > _congestionFrameCount)
                    {
                        _congestion = true;

                        Console.WriteLine("Congestion: ON");
                    }
                    else if (_congestion && _frames.Count < _congestionFrameCount / 2)
                    {
                        _congestion = false;

                        Console.WriteLine("Congestion: OFF");
                    }

                    if (_frames.Count > _maxFrameCount)
                    {
                        throw new Exception(@"System can't keep up with the Recording. Frames are not being written. Retry again or try with a smaller region, lower Frame Rate or another Codec.");
                    }

                    var timestamp = DateTime.Now;

                    if (task != null)
                    {
                        // If false, stop recording
                        if (!await task)
                            return;

                        if (!_congestion)
                        {
                            var requiredFrames = _sw.Elapsed.TotalSeconds * _frameRate;
                            var diff = requiredFrames - frameCount;

                            for (var i = 0; i < diff; ++i)
                            {
                                if (!AddFrame(RepeatFrame.Instance))
                                    return;
                            }
                        }
                    }

                    task = Task.Factory
                        .StartNew(() => _imageProvider.Capture())
                        .ContinueWith(T => AddFrame(T.Result));

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

                        Dispose(false, true);
                    }
                }
            }
        }

        void AudioProvider_DataAvailable(object Sender, DataAvailableEventArgs E)
        {
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

                            Dispose(true, true);
                        }
                    }
                }
            }
        }

        #region Dispose
        void Dispose(bool TerminateRecord, bool TerminateWrite)
        {
            if (_disposed)
                return;

            _disposed = true;

            _audioProvider?.Stop();
            _audioProvider?.Dispose();

            if (_videoWriter != null)
            {
                _frames.CompleteAdding();

                _continueCapturing.Set();

                if (TerminateRecord)
                    _recordTask.Wait();

                if (TerminateWrite)
                    _writeTask.Wait();

                _videoWriter.Dispose();
                _frames.Dispose();

                _continueCapturing.Dispose();
            }
            else _audioWriter.Dispose();

            _imageProvider?.Dispose();
        }

        /// <summary>
        /// Frees all resources used by this instance.
        /// </summary>
        public void Dispose()
        {
            lock (_syncLock)
            {
                Dispose(true, true);
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
