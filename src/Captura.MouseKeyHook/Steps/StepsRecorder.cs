using System;
using System.Drawing;
using System.Reactive;
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
        readonly KeymapViewModel _keymap;

        IRecordStep _lastStep;
        Point? _dragStartPoint;

        IObservable<IRecordStep> Observe(IMouseKeyHook Hook, CancellationToken CancellationToken, out IObservable<Unit> ShotObservable)
        {
            var subject = new Subject<IRecordStep>();
            var shotSubject = new Subject<Unit>();
            ShotObservable = shotSubject;

            void OnNext(IRecordStep NextStep)
            {
                if (_lastStep != null)
                {
                    if (_lastStep.Merge(NextStep))
                        return;

                    subject.OnNext(_lastStep);
                }

                shotSubject.OnNext(Unit.Default);

                _lastStep = NextStep;
            }

            Hook.MouseClick += (S, E) =>
            {
                // HACK: Prevent Mouse click generated due to dragging
                if (_dragStartPoint != null)
                    return;

                OnNext(new MouseClickStep(_mouseClickSettings, E));
            };

            Hook.MouseDoubleClick += (S, E) => OnNext(new MouseClickStep(_mouseClickSettings, E));
            Hook.MouseDragStarted += (S, E) => _dragStartPoint = E.Location;
            Hook.MouseDragFinished += (S, E) =>
            {
                OnNext(new MouseDragStep(_dragStartPoint.Value, E.Location, _mouseClickSettings));

                _dragStartPoint = null;
            };

            // TODO: Event is not firing for touchpad scroll
            Hook.MouseWheel += (S, E) => OnNext(new ScrollStep(E, _mouseClickSettings));

            Hook.KeyDown += (S, E) => OnNext(new KeyStep(_keystrokesSettings, E, _keymap));

            CancellationToken.Register(() =>
            {
                shotSubject.OnCompleted();

                subject.OnNext(_lastStep);

                subject.OnCompleted();
            });

            return subject
                .Where(M => _recording);
        }

        public StepsRecorder(IMouseKeyHook Hook,
            IVideoFileWriter VideoWriter,
            IImageProvider ImageProvider,
            MouseClickSettings MouseClickSettings,
            KeystrokesSettings KeystrokesSettings,
            KeymapViewModel KeymapViewModel)
        {
            _hook = Hook;
            _videoWriter = VideoWriter;
            _imageProvider = ImageProvider;
            _mouseClickSettings = MouseClickSettings;
            _keystrokesSettings = KeystrokesSettings;
            _keymap = KeymapViewModel;

            _recordTask = Task.Factory.StartNew(DoRecord, TaskCreationOptions.LongRunning);
        }

        void DoRecord()
        {
            var observer = Observe(_hook, _cancellationTokenSource.Token, out var shot);

            var frames = shot.Select(M => _imageProvider.Capture())
                .Zip(observer, (Frame, Step) =>
                {
                    Step.Draw(Frame, _imageProvider.PointTransform);

                    return Frame.GenerateFrame();
                });

            foreach (var frame in frames.ToEnumerable())
            {
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