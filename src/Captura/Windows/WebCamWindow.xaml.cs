using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using Captura.ViewModels;
using Screna;

namespace Captura
{
    public partial class WebCamWindow
    {
        readonly WebcamModel _webcamModel;
        readonly ScreenShotModel _screenShotModel;
        readonly IPlatformServices _platformServices;

        public WebCamWindow(WebcamModel WebcamModel,
            ScreenShotModel ScreenShotModel,
            IPlatformServices PlatformServices)
        {
            _webcamModel = WebcamModel;
            _screenShotModel = ScreenShotModel;
            _platformServices = PlatformServices;

            InitializeComponent();
            
            Closing += (S, E) =>
            {
                Hide();

                E.Cancel = true;
            };
        }

        public static WebCamWindow Instance { get; } = ServiceProvider.Get<WebCamWindow>();

        void CloseButton_Click(object Sender, RoutedEventArgs E) => Close();
        
        async void CaptureImage_OnClick(object Sender, RoutedEventArgs E)
        {
            try
            {
                var img = _webcamModel.WebcamCapture?.Capture(GraphicsBitmapLoader.Instance);

                await _screenShotModel.SaveScreenShot(img);
            }
            catch { }
        }

        public void SetupWebcamPreview()
        {
            // Open Preview Window
            _webcamModel.PreviewClicked += this.ShowAndFocus;

            WebCameraControl.IsVisibleChanged += (S, E) => SwitchWebcamPreview();

            void OnSizeChange()
            {
                var rect = GetPreviewWindowRect();

                _webcamModel.WebcamCapture?.UpdatePreview(null, rect);
            }

            WebCameraControl.SizeChanged += (S, E) => OnSizeChange();

            _webcamModel.PropertyChanged += (S, E) =>
            {
                if (E.PropertyName == nameof(WebcamModel.SelectedCam) && _webcamModel.WebcamCapture != null)
                {
                    SwitchWebcamPreview();
                }
            };
        }

        Rectangle GetPreviewWindowRect()
        {
            var rect = new RectangleF(5, 40, (float)WebCameraControl.ActualWidth, (float)WebCameraControl.ActualHeight);

            return rect.ApplyDpi();
        }

        void SwitchWebcamPreview()
        {
            if (_webcamModel.WebcamCapture == null)
                return;

            if (WebCameraControl.IsVisible)
            {
                if (PresentationSource.FromVisual(WebCameraControl) is HwndSource source)
                {
                    var win = _platformServices.GetWindow(source.Handle);

                    var rect = GetPreviewWindowRect();

                    _webcamModel.WebcamCapture.UpdatePreview(win, rect);
                }
            }
            else if (PresentationSource.FromVisual(MainWindow.Instance) is HwndSource source)
            {
                var win = _platformServices.GetWindow(source.Handle);

                var rect = new RectangleF(280, 1, 50, 40).ApplyDpi();

                _webcamModel.WebcamCapture.UpdatePreview(win, rect);
            }
        }
    }
}