using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using Captura.ViewModels;
using Screna;

namespace Captura
{
    public partial class WebCamWindow
    {
        WebCamWindow()
        {
            InitializeComponent();
            
            Closing += (S, E) =>
            {
                Hide();

                E.Cancel = true;
            };
        }

        public static WebCamWindow Instance { get; } = new WebCamWindow();

        WebcamControl GetWebCamControl() => WebCameraControl;

        void CloseButton_Click(object Sender, RoutedEventArgs E) => Close();
        
        async void CaptureImage_OnClick(object Sender, RoutedEventArgs E)
        {
            try
            {
                var img = ServiceProvider.Get<WebcamModel>().WebcamCapture?.Capture(GraphicsBitmapLoader.Instance);

                await ServiceProvider.Get<ScreenShotModel>().SaveScreenShot(img);
            }
            catch { }
        }

        public void SetupWebcamPreview()
        {
            var webcamModel = ServiceProvider.Get<WebcamModel>();
            var camControl = GetWebCamControl();

            // Open Preview Window
            webcamModel.PreviewClicked += this.ShowAndFocus;

            camControl.IsVisibleChanged += (S, E) => SwitchWebcamPreview();

            void OnSizeChange()
            {
                var rect = GetPreviewWindowRect();

                webcamModel.WebcamCapture?.UpdatePreview(null, rect);
            }

            camControl.SizeChanged += (S, E) => OnSizeChange();

            webcamModel.PropertyChanged += (S, E) =>
            {
                if (E.PropertyName == nameof(WebcamModel.SelectedCam) && webcamModel.WebcamCapture != null)
                {
                    SwitchWebcamPreview();
                }
            };
        }

        Rectangle GetPreviewWindowRect()
        {
            var camControl = GetWebCamControl();

            var rect = new RectangleF(5, 40, (float)camControl.ActualWidth, (float)camControl.ActualHeight);

            return rect.ApplyDpi();
        }

        void SwitchWebcamPreview()
        {
            var webcamModel = ServiceProvider.Get<WebcamModel>();
            var camControl = GetWebCamControl();
            var mainWindow = MainWindow.Instance;

            if (webcamModel.WebcamCapture == null)
                return;

            var platformServices = ServiceProvider.Get<IPlatformServices>();

            if (camControl.IsVisible)
            {
                if (PresentationSource.FromVisual(camControl) is HwndSource source)
                {
                    var win = platformServices.GetWindow(source.Handle);

                    var rect = GetPreviewWindowRect();

                    webcamModel.WebcamCapture.UpdatePreview(win, rect);
                }
            }
            else if (PresentationSource.FromVisual(mainWindow) is HwndSource source)
            {
                var win = platformServices.GetWindow(source.Handle);

                var rect = new RectangleF(280, 1, 50, 40).ApplyDpi();

                webcamModel.WebcamCapture.UpdatePreview(win, rect);
            }
        }
    }
}