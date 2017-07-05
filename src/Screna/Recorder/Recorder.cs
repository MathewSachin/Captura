using Screna.Audio;
using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

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

        readonly int _frameRate;

        readonly BlockingCollection<object> _frames = new BlockingCollection<object>();

        readonly ManualResetEvent _continueCapturing;

        readonly Task _writeTask, _recordTask;
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

            _frameRate = FrameRate;

            _continueCapturing = new ManualResetEvent(false);

            if (VideoWriter.SupportsAudio && AudioProvider != null)
                AudioProvider.DataAvailable += AudioProvider_DataAvailable;
            else _audioProvider = null;

            _recordTask = Task.Factory.StartNew(DoRecord, TaskCreationOptions.LongRunning);
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

            _audioProvider.DataAvailable += (s, e) => _audioWriter.Write(e.Buffer, 0, e.Length);
        }

        void DoWrite()
        {
            try
            {
                while (!_frames.IsCompleted)
                {
                    _frames.TryTake(out var data, -1);

                    switch (data)
                    {
                        case Bitmap img:
                            _videoWriter.WriteFrame(img);
                            break;

                        case DataAvailableEventArgs args:
                            _videoWriter.WriteAudio(args.Buffer, args.Length);
                            break;
                    }
                }
            }
            catch (Exception E)
            {
                ErrorOccured?.Invoke(E);

                Dispose(true, false);
            }
        }

        void DoRecord()
        {
            try
            {
                var frameInterval = TimeSpan.FromSeconds(1.0 / _frameRate);

                while (_continueCapturing.WaitOne() && !_frames.IsAddingCompleted)
                {
                    var timestamp = DateTime.Now;

                    try { _frames.Add(_imageProvider.Capture()); }
                    catch { }

                    var timeTillNextFrame = timestamp + frameInterval - DateTime.Now;

                    if (timeTillNextFrame > TimeSpan.Zero)
                        Thread.Sleep(timeTillNextFrame);
                }
            }
            catch (Exception E)
            {
                ErrorOccured?.Invoke(E);

                Dispose(false, true);
            }
        }

        void AudioProvider_DataAvailable(object sender, DataAvailableEventArgs e)
        {
            try { _frames.Add(e); }
            catch { }
        }

        #region Dispose
        void Dispose(bool TerminateRecord, bool TerminateWrite)
        {
            ThrowIfDisposed();

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

                _continueCapturing.Close();
            }
            else _audioWriter.Dispose();

            _imageProvider?.Dispose();

            _disposed = true;
        }

        /// <summary>
        /// Frees all resources used by this instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true, true);
        }

        bool _disposed;

        /// <summary>
        /// Fired when an error occurs
        /// </summary>
        public event Action<Exception> ErrorOccured;

        void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException("this");
        }
        #endregion

        /// <summary>
        /// Start Recording.
        /// </summary>
        public void Start()
        {
            ThrowIfDisposed();

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
        }
    }
}
