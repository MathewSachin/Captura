using System;
using System.Collections.ObjectModel;
using System.Drawing;
using Captura.Webcam;

namespace Captura.Models
{
    public class WebCamProvider : NotifyPropertyChanged, IWebCamProvider
    {
        public WebCamProvider()
        {
            _camControl = WebCamWindow.Instance.GetWebCamControl();
            
            Refresh();
        }
        
        public ObservableCollection<IWebcamItem> AvailableCams { get; } = new ObservableCollection<IWebcamItem>();

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
                        ServiceProvider.MessageProvider.ShowError($"Could not Start Webcam.\n\n\n{e}");
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

            foreach (var cam in CaptureWebcam.VideoInputDevices)
                AvailableCams.Add(new WebcamItem(cam));

            SelectedCam = WebcamItem.NoWebcam;
        }

        public Bitmap Capture()
        {
            try
            {
                return _camControl.Dispatcher.Invoke(() => _camControl.Capture.GetFrame());
            }
            catch { return null; }
        }
    }
}
