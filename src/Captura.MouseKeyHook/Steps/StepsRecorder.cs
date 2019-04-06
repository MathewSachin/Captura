using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace Captura.Models
{
    public class StepsRecorder : IRecorder
    {
        readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        readonly IMouseKeyHook _hook;
        IVideoFileWriter _videoWriter;
        IImageProvider _imageProvider;
        readonly Task _recordTask;
        volatile bool _recording;
        readonly MouseClickSettings _mouseClickSettings;
        readonly KeystrokesSettings _keystrokesSettings;

        IObservable<IOverlay> Observe(IMouseKeyHook Hook, CancellationToken CancellationToken)
        {
            var subject = new Subject<IOverlay>();

            Hook.MouseClick += (S, E) => subject.OnNext(new MouseClickStep(_mouseClickSettings, E));
            Hook.MouseDoubleClick += (S, E) => subject.OnNext(new MouseClickStep(_mouseClickSettings, E));

            CancellationToken.Register(() => subject.OnCompleted());

            // TODO: Define interface with Merge support for merging related Steps
            return subject
                .Where(M => _recording);
        }

        public StepsRecorder(IMouseKeyHook Hook,
            IVideoFileWriter VideoWriter,
            IImageProvider ImageProvider,
            MouseClickSettings MouseClickSettings,
            KeystrokesSettings KeystrokesSettings)
        {
            _hook = Hook;
            _videoWriter = VideoWriter;
            _imageProvider = ImageProvider;
            _mouseClickSettings = MouseClickSettings;
            _keystrokesSettings = KeystrokesSettings;

            _recordTask = Task.Factory.StartNew(DoRecord, TaskCreationOptions.LongRunning);
        }

        void DoRecord()
        {
            var observer = Observe(_hook, _cancellationTokenSource.Token);

            foreach (var overlay in observer.ToEnumerable())
            {
                var editableFrame = _imageProvider.Capture();

                overlay.Draw(editableFrame, _imageProvider.PointTransform);

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

        public void Start() => _recording = true;

        public void Stop() => _recording = false;

        public event Action<Exception> ErrorOccurred;

        public void Dispose()
        {
            _recording = false;

            _hook.Dispose();
            _cancellationTokenSource.Cancel();

            _recordTask.Wait();

            _videoWriter.Dispose();
            _videoWriter = null;

            _imageProvider?.Dispose();
            _imageProvider = null;
        }
    }
}