using System;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable MethodSupportsCancellation

namespace Captura.Models
{
    public class StepsRecorder : IRecorder
    {
        readonly IMouseKeyHook _hook;
        IVideoFileWriter _videoWriter;
        IImageProvider _imageProvider;

        readonly AutoResetEvent _captureEvent;
        readonly ManualResetEvent _continueCapturing;
        readonly CancellationTokenSource _cancellationTokenSource;
        readonly CancellationToken _cancellationToken;

        readonly Task _recordTask;

        public StepsRecorder(IMouseKeyHook Hook,
            IVideoFileWriter VideoWriter,
            IImageProvider ImageProvider)
        {
            _hook = Hook;
            _videoWriter = VideoWriter;
            _imageProvider = ImageProvider;

            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;

            _captureEvent = new AutoResetEvent(true);
            _continueCapturing = new ManualResetEvent(false);

            Hook.KeyUp += HookCallback;
            Hook.KeyDown += HookCallback;
            Hook.MouseUp += HookCallback;
            Hook.MouseDown += HookCallback;

            _recordTask = Task.Factory.StartNew(DoRecord, TaskCreationOptions.LongRunning);
        }

        void HookCallback<T>(object Sender, T Args)
        {
            _captureEvent.Set();
        }

        void DoRecord()
        {
            bool CanContinue()
            {
                try
                {
                    return WaitHandle.WaitAll(new WaitHandle[]
                    {
                        _continueCapturing,
                        _captureEvent
                    });
                }
                catch (ObjectDisposedException)
                {
                    return false;
                }
            }

            while (CanContinue() && !_cancellationToken.IsCancellationRequested)
            {
                var editableFrame = _imageProvider.Capture();

                var frame = editableFrame.GenerateFrame();

                try
                {
                    _videoWriter.WriteFrame(frame);
                }
                catch (InvalidOperationException)
                {
                    break;
                }
            }
        }

        public void Start()
        {
            _continueCapturing?.Set();
        }

        public void Stop()
        {
            _continueCapturing?.Reset();
        }

        public event Action<Exception> ErrorOccurred;

        public void Dispose()
        {
            _hook.Dispose();
            _cancellationTokenSource.Cancel();

            // Resume record loop if paused so it can exit
            _continueCapturing.Set();
            _captureEvent.Set();

            _recordTask.Wait();

            _videoWriter.Dispose();
            _videoWriter = null;

            _continueCapturing.Dispose();
            _captureEvent.Dispose();

            _imageProvider?.Dispose();
            _imageProvider = null;
        }
    }
}