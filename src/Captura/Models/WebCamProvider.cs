using System;
using System.Collections.ObjectModel;
using WebEye.Controls.Wpf;

namespace Captura.Models
{
    public class WebCamProvider : NotifyPropertyChanged, IWebCamProvider
    {
        readonly WebCamWindow _window;

        public WebCamProvider()
        {
            _window = WebCamWindow.Instance;

            _camControl = _window.GetWebCamControl();

            _window.IsVisibleChanged += (s, e) =>
            {
                if (!_window.IsVisible)
                    Dispose();
            };
            
            Refresh();
        }
        
        public ObservableCollection<IWebcamItem> AvailableCams { get; } = new ObservableCollection<IWebcamItem>();

        readonly WebCameraControl _camControl;

        IWebcamItem _selectedCam = WebcamItem.NoWebcam;

        public IWebcamItem SelectedCam
        {
            get => _selectedCam;
            set
            {
                if (_selectedCam == value)
                    return;

                _selectedCam = value;
                
                if (_camControl.IsCapturing)
                    _camControl.StopCapture();

                if (_selectedCam == null || _selectedCam == WebcamItem.NoWebcam)
                {
                    _window.Hide();
                }
                else
                {
                    _window.Show();

                    try
                    {
                        if (value is WebcamItem model)
                        {

                            _camControl.StartCapture(model.Cam);

                            _selectedCam = value;

                            OnPropertyChanged();
                        }
                    }
                    catch (Exception E)
                    {
                        ServiceProvider.MessageProvider.ShowError($"Could not Start Capture\n\n\n{E}");

                        _window.Hide();
                    }
                }

                OnPropertyChanged();
            }
        }
        
        public void Refresh()
        {
            AvailableCams.Clear();

            AvailableCams.Add(WebcamItem.NoWebcam);

            if (_camControl == null)
                return;

            foreach (var cam in _camControl.GetVideoCaptureDevices())
                AvailableCams.Add(new WebcamItem(cam));

            SelectedCam = WebcamItem.NoWebcam;
        }

        public void Dispose()
        {
            SelectedCam = WebcamItem.NoWebcam;
        }
    }
}
