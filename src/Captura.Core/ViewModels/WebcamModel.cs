using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Captura.Models;
using Reactive.Bindings;

namespace Captura.Webcam
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class WebcamModel : NotifyPropertyChanged
    {
        readonly IWebCamProvider _webcamProvider;

        readonly SyncContextManager _syncContext = new SyncContextManager();

        public WebcamModel(IWebCamProvider WebcamProvider)
        {
            _webcamProvider = WebcamProvider;

            AvailableCams = new ReadOnlyObservableCollection<IWebcamItem>(_cams);

            RefreshCommand = new DelegateCommand(Refresh);

            Refresh();
        }

        readonly ObservableCollection<IWebcamItem> _cams = new ObservableCollection<IWebcamItem>();

        public ReadOnlyObservableCollection<IWebcamItem> AvailableCams { get; }

        public void Refresh()
        {
            var lastWebcamName = SelectedCam?.Name;

            _cams.Clear();

            _cams.Add(NoWebcamItem.Instance);

            foreach (var cam in _webcamProvider.GetSources())
                _cams.Add(cam);

            var matchingWebcam = AvailableCams.FirstOrDefault(M => M.Name == lastWebcamName);

            SelectedCam = matchingWebcam ?? NoWebcamItem.Instance;
        }

        IWebcamItem _selectedCam = NoWebcamItem.Instance;

        public ICommand RefreshCommand { get; }

        public IWebcamItem SelectedCam
        {
            get => _selectedCam;
            set
            {
                if (_selectedCam == value)
                    return;

                ReleaseCaptureInternal();

                _selectedCam = value;

                if (_acquireCount.Value > 0)
                {
                    InitCaptureInternal();
                }

                OnPropertyChanged();
            }
        }

        readonly ReactivePropertySlim<IWebcamCapture> _webcamCapture = new ReactivePropertySlim<IWebcamCapture>();

        readonly IReactiveProperty<int> _acquireCount = new ReactivePropertySlim<int>();

        public IReadOnlyReactiveProperty<IWebcamCapture> InitCapture()
        {
            ++_acquireCount.Value;

            if (_acquireCount.Value == 1)
            {
                InitCaptureInternal();
            }

            return _webcamCapture;
        }

        void InitCaptureInternal()
        {
            _syncContext.Run(() => _webcamCapture.Value = SelectedCam?.BeginCapture(() => PreviewClicked?.Invoke()));
        }

        void ReleaseCaptureInternal()
        {
            _syncContext.Run(() =>
            {
                _webcamCapture.Value?.Dispose();
                _webcamCapture.Value = null;
            });
        }

        public void ReleaseCapture()
        {
            --_acquireCount.Value;

            if (_acquireCount.Value == 0)
            {
                ReleaseCaptureInternal();
            }
        }

        public event Action PreviewClicked;
    }
}