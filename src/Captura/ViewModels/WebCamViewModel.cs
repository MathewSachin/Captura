using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using WebEye.Controls.Wpf;

namespace Captura.ViewModels
{
    public class WebCamViewModel : ViewModelBase, IDisposable
    {
        public ObservableCollection<WebCameraId> AvailableCameras { get; } = new ObservableCollection<WebCameraId>();

        public WebCamViewModel()
        {
            RefreshCommand = new DelegateCommand(Refresh);
        }

        WebCameraControl _camControl;

        public void Init(WebCameraControl CamControl)
        {
            _camControl = CamControl;

            Refresh();
        }
        
        WebCameraId _selectedCamera;

        public WebCameraId SelectedCamera
        {
            get { return _selectedCamera; }
            set
            {
                if (value == null || SelectedCamera == value)
                    return;

                if (_camControl.IsCapturing)
                    _camControl.StopCapture();

                try
                {
                    _camControl.StartCapture(value);

                    _selectedCamera = value;
                }
                finally
                {
                    OnPropertyChanged();
                }
            }
        }

        public ICommand RefreshCommand { get; }

        public void Refresh()
        {
            AvailableCameras.Clear();

            if (_camControl == null)
                return;

            foreach (var cam in _camControl.GetVideoCaptureDevices())
                AvailableCameras.Add(cam);
        }

        public void Dispose()
        {
            _selectedCamera = null;

            if (_camControl.IsCapturing)
                _camControl.StopCapture();
        }
    }
}