using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;
using Captura.Webcam;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class CoreWebCamProvider : NotifyPropertyChanged, IWebCamProvider
    {
        public CoreWebCamProvider()
        {
            AvailableCams = new ReadOnlyObservableCollection<IWebcamItem>(_cams);

            _previewForm = new Form();
            
            Refresh();
        }

        readonly Form _previewForm;

        CaptureWebcam _captureWebcam;

        readonly ObservableCollection<IWebcamItem> _cams = new ObservableCollection<IWebcamItem>();

        public ReadOnlyObservableCollection<IWebcamItem> AvailableCams { get; }
        
        IWebcamItem _selectedCam = WebcamItem.NoWebcam;

        public IWebcamItem SelectedCam
        {
            get => _selectedCam;
            set
            {
                if (_selectedCam == value)
                    return;

                _selectedCam = value;

                if (_captureWebcam != null)
                {
                    _captureWebcam.StopPreview();
                    _captureWebcam.Dispose();

                    _captureWebcam = null;
                }

                if (value is WebcamItem model)
                {
                    try
                    {
                        _captureWebcam = new CaptureWebcam(model.Cam, null)
                        {
                            PreviewWindow = _previewForm.Handle
                        };

                        _captureWebcam.StartPreview();

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

            foreach (var cam in CaptureWebcam.VideoInputDevices)
                _cams.Add(new WebcamItem(cam));

            SelectedCam = WebcamItem.NoWebcam;
        }

        public IDisposable Capture(IBitmapLoader BitmapLoader)
        {
            try
            {
                return _captureWebcam?.GetFrame(BitmapLoader);
            }
            catch { return null; }
        }

        public int Width => _captureWebcam?.Size.Width ?? 0;

        public int Height => _captureWebcam?.Size.Height ?? 0;
    }
}
