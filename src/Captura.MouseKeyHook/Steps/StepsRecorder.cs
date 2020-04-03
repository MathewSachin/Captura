using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Captura.Video;

namespace Captura.MouseKeyHook.Steps
{
    public class StepsRecorder : IRecorder
    {
        readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        readonly IMouseKeyHook _hook;
        IVideoFileWriter _videoWriter;
        IImageProvider _imageProvider;
        readonly Task _recordTask;
        volatile bool _recording;
        readonly StepsSettings _stepsSettings;
        readonly MouseClickSettings _mouseClickSettings;
        readonly KeystrokesSettings _keystrokesSettings;
        readonly KeymapViewModel _keymap;
        bool _modifierSingleDown;

        IRecordStep _lastStep;

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
                var step = new MouseClickStep(_mouseClickSettings,
                    _keystrokesSettings, E,
                    _keymap);

                OnNext(step);
            };

            Hook.MouseDoubleClick += (S, E) =>
            {
                var step = new MouseClickStep(_mouseClickSettings,
                    _keystrokesSettings, E,
                    _keymap);

                OnNext(step);
            };

            Hook.MouseDragStarted += (S, E) => 
            {
                var step = new MouseDragBeginStep(E.Location,
                    _mouseClickSettings,
                    _keystrokesSettings,
                    _keymap);

                OnNext(step);
            };

            Hook.MouseDragFinished += (S, E) =>
            {
                var step = new MouseDragStep(E.Location,
                    _mouseClickSettings,
                    _keystrokesSettings,
                    _keymap);

                OnNext(step);
            };

            if (_stepsSettings.IncludeScrolls)
            {
                // TODO: Event is not firing for touchpad scroll
                Hook.MouseWheel += (S, E) =>
                {
                    var step = new ScrollStep(E,
                        _mouseClickSettings,
                        _keystrokesSettings,
                        _keymap);

                    OnNext(step);
                };
            }

            Hook.KeyDown += (S, E) =>
            {
                _modifierSingleDown = false;

                var record = new KeyRecord(E, _keymap);

                var display = record.Display;

                if (display == _keymap.Control
                    || display == _keymap.Alt
                    || display == _keymap.Shift)
                {
                    _modifierSingleDown = true;
                }
                else OnNext(new KeyStep(_keystrokesSettings, record));
            };

            Hook.KeyUp += (S, E) =>
            {
                var record = new KeyRecord(E, _keymap);

                var display = record.Display;

                if (display == _keymap.Control
                    || display == _keymap.Alt
                    || display == _keymap.Shift)
                {
                    if (_modifierSingleDown)
                    {
                        OnNext(new KeyStep(_keystrokesSettings, record));
                    }
                }
            };

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
            StepsSettings StepsSettings,
            KeymapViewModel KeymapViewModel)
        {
            _hook = Hook;
            _videoWriter = VideoWriter;
            _imageProvider = ImageProvider;
            _stepsSettings = StepsSettings;
            _mouseClickSettings = MouseClickSettings;
            _keystrokesSettings = KeystrokesSettings;
            _keymap = KeymapViewModel;

            var stepsObservable = Observe(_hook, _cancellationTokenSource.Token, out var shotObservable);

            _recordTask = Task.Factory.StartNew(() => DoRecord(stepsObservable, shotObservable), TaskCreationOptions.LongRunning);
        }

        void DoRecord(IObservable<IRecordStep> StepsObservable, IObservable<Unit> ShotObservable)
        {
            var frames = ShotObservable.Select(M => _imageProvider.Capture())
                .Zip(StepsObservable, (Frame, Step) =>
                {
                    Step.Draw(Frame, _imageProvider.PointTransform);

                    return Frame.GenerateFrame(TimeSpan.Zero);
                });

            foreach (var frame in frames.ToEnumerable())
            {
                _videoWriter.WriteFrame(frame);
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