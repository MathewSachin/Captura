using System;
using System.Collections.ObjectModel;
using Captura.Webcam;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class WebCamProvider : NotifyPropertyChanged, IWebCamProvider
    {
        public WebCamProvider()
        {
            AvailableCams = new ReadOnlyObservableCollection<IWebcamItem>(_cams);

            _camControl = WebCamWindow.Instance.GetWebCamControl();
            
            Refresh();
        }
        
        readonly ObservableCollection<IWebcamItem> _cams = new ObservableCollection<IWebcamItem>();

        public ReadOnlyObservableCollection<IWebcamItem> AvailableCams { get; }

        readonly WebcamControl _camControl;

        IWebcamItem _selectedCam = WebcamItem.NoWebcam;

        public IWebcamItem SelectedCam
        {
            get => _selectedCam;
            set
            {
                if (_selectedCam == value)
                    return;

                _selectedCam = value;

                _camControl.Capture?.StopPreview();

                if (value is WebcamItem model)
                {
                    try
                    {
                        _camControl.VideoDevice = model.Cam;

                        if (_camControl.IsVisible)
                            _camControl.Refresh();
                        else _camControl.ShowOnMainWindow(MainWindow.Instance);

                        _selectedCam = value;

                        OnPropertyChanged();
                    }
                    catch (Exception e)
                    {
                        ServiceProvider.MessageProvider.ShowException(e, "Could not Start Webcam");
                    }
                }

                OnPropertyChanged();
            }
        }
        
        public void Refresh()
        {
            _cams.Clear();

            _cams.Add(WebcamItem.NoWebcam);

            if (_camControl == null)
                return;

            foreach (var cam in Filter.VideoInputDevices)
                _cams.Add(new WebcamItem(cam));

            SelectedCam = WebcamItem.NoWebcam;
        }

        public IDisposable Capture(IBitmapLoader BitmapLoader)
        {
            try
            {
                return _camControl.Dispatcher.Invoke(() => _camControl.Capture?.GetFrame(BitmapLoader));
            }
            catch { return null; }
        }

        public int Width => _camControl.Dispatcher.Invoke(() => _camControl.Capture?.Size.Width ?? 0);

        public int Height => _camControl.Dispatcher.Invoke(() => _camControl.Capture?.Size.Height ?? 0);
    }
}
