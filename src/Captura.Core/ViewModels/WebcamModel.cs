using System;
using System.Collections.ObjectModel;
using System.Linq;
using Captura.Models;

namespace Captura.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class WebcamModel : NotifyPropertyChanged, IRefreshable
    {
        readonly IWebCamProvider _webcamProvider;

        public WebcamModel(IWebCamProvider WebcamProvider)
        {
            _webcamProvider = WebcamProvider;

            AvailableCams = new ReadOnlyObservableCollection<IWebcamItem>(_cams);

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

        public IWebcamItem SelectedCam
        {
            get => _selectedCam;
            set
            {
                if (_selectedCam == value)
                    return;

                WebcamCapture?.Dispose();

                _selectedCam = value;

                WebcamCapture = _selectedCam?.BeginCapture(() => PreviewClicked?.Invoke());

                OnPropertyChanged();
            }
        }

        public event Action PreviewClicked;

        public IWebcamCapture WebcamCapture { get; private set; }
    }
}