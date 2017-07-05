using System;
using System.Threading;
using System.Threading.Tasks;

namespace Screna
{
    /// <summary>
    /// An <see cref="IRecorder"/> which records to a Gif using Delay for each frame instead of Frame Rate.
    /// </summary>
    public class VFRGifRecorder : IRecorder
    {
        #region Fields
        readonly GifWriter _videoEncoder;
        readonly IImageProvider _imageProvider;

        readonly Task _recordTask;

        readonly ManualResetEvent _stopCapturing = new ManualResetEvent(false),
            _continueCapturing = new ManualResetEvent(false);
        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="VFRGifRecorder"/>.
        /// </summary>
        /// <param name="Encoder">The <see cref="GifWriter"/> to write into.</param>
        /// <param name="ImageProvider">The <see cref="IImageProvider"/> providing the individual frames.</param>
        /// <exception cref="ArgumentNullException"><paramref name="Encoder"/> or <paramref name="ImageProvider"/> is null.</exception>
        public VFRGifRecorder(GifWriter Encoder, IImageProvider ImageProvider)
        {
            // Init Fields
            _imageProvider = ImageProvider ?? throw new ArgumentNullException(nameof(ImageProvider));
            _videoEncoder = Encoder ?? throw new ArgumentNullException(nameof(Encoder));
            
            // Not Actually Started, Waits for _continueCapturing to be Set
            _recordTask = Task.Factory.StartNew(Record);
        }

        /// <summary>
        /// Start recording.
        /// </summary>
        public void Start() => _continueCapturing.Set();

        void Dispose(bool ErrorOccured)
        {
            // Resume if Paused
            _continueCapturing?.Set();
            
            _stopCapturing.Set();

            if (!ErrorOccured)
                _recordTask.Wait();

            _imageProvider.Dispose();

            _videoEncoder.Dispose();
        }

        /// <summary>
        /// Frees resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(false);
        }

        /// <summary>
        /// Override this method with the code to pause recording.
        /// </summary>
        public void Stop() => _continueCapturing.Reset();

        void Record()
        {
            try
            {
                var lastFrameWriteTime = DateTime.MinValue;
                
                while (!_stopCapturing.WaitOne(0) && _continueCapturing.WaitOne())
                {
                    var frame = _imageProvider.Capture();

                    var delay = lastFrameWriteTime == DateTime.MinValue ? 0
                        : (int)(DateTime.Now - lastFrameWriteTime).TotalMilliseconds;

                    lastFrameWriteTime = DateTime.Now;

                    _videoEncoder.WriteFrame(frame, delay);
                }
            }
            catch (Exception E)
            {
                ErrorOccured?.Invoke(E);

                Dispose(true);
            }
        }

        /// <summary>
        /// Fired when an Error occurs.
        /// </summary>
        public event Action<Exception> ErrorOccured;
    }
}
