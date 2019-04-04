using System;
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

        IObservable<Unit> Observe(IMouseKeyHook Hook, CancellationToken CancellationToken)
        {
            var subject = new Subject<Unit>();

            void Callback<T>(object S, T Args) => subject.OnNext(Unit.Default);

            Hook.KeyUp += Callback;
            Hook.KeyDown += Callback;
            Hook.MouseUp += Callback;
            Hook.MouseDown += Callback;

            CancellationToken.Register(() =>
            {
                Hook.KeyUp -= Callback;
                Hook.KeyDown -= Callback;
                Hook.MouseUp -= Callback;
                Hook.MouseDown -= Callback;

                subject.OnCompleted();
            });

            return subject
                .Where(M => _recording)
                .Throttle(TimeSpan.FromMilliseconds(70));
        }

        public StepsRecorder(IMouseKeyHook Hook,
            IVideoFileWriter VideoWriter,
            IImageProvider ImageProvider)
        {
            _hook = Hook;
            _videoWriter = VideoWriter;
            _imageProvider = ImageProvider;

            _recordTask = Task.Factory.StartNew(DoRecord, TaskCreationOptions.LongRunning);
        }

        void DoRecord()
        {
            var observer = Observe(_hook, _cancellationTokenSource.Token);

            foreach (var _ in observer.ToEnumerable())
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