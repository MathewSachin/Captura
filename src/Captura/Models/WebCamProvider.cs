using System;
using System.Collections.ObjectModel;
using WebEye.Controls.Wpf;

namespace Captura.Models
{
    public class WebCamProvider : NotifyPropertyChanged, IWebCamProvider
    {
        class WebCamModel
        {
            readonly string _name;

            public WebCameraId Cam { get; }

            public WebCamModel(WebCameraId Cam, string Name)
            {
                this.Cam = Cam;

                _name = Name;
            }

            public override string ToString() => _name;
        }

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

        const string None = "No WebCam";

        public ObservableCollection<object> AvailableCams { get; } = new ObservableCollection<object>();

        readonly WebCameraControl _camControl;

        object _selectedCam = None;

        public object SelectedCam
        {
            get => _selectedCam;
            set
            {
                if (_selectedCam == value)
                    return;

                _selectedCam = value;
                
                if (_camControl.IsCapturing)
                    _camControl.StopCapture();

                if (_selectedCam.ToString() == None)
                {
                    _window.Hide();
                }
                else
                {
                    _window.Show();

                    try
                    {
                        var model = value as WebCamModel;

                        _camControl.StartCapture(model.Cam);

                        _selectedCam = value;

                        OnPropertyChanged();
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

            AvailableCams.Add(None);

            if (_camControl == null)
                return;

            foreach (var cam in _camControl.GetVideoCaptureDevices())
                AvailableCams.Add(new WebCamModel(cam, cam.Name));

            SelectedCam = None;
        }

        public void Dispose()
        {
            SelectedCam = None;
        }
    }
}
