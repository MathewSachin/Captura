using System.Windows;

namespace Captura
{
    public partial class WebCamWindow
    {
        WebCamWindow()
        {
            InitializeComponent();
            
            Closing += (s, e) =>
            {
                Hide();

                e.Cancel = true;
            };
        }

        public static WebCamWindow Instance { get; } = new WebCamWindow();

        public WebcamControl GetWebCamControl() => webCameraControl;

        void CloseButton_Click(object Sender, RoutedEventArgs E) => Close();
        
        void CaptureImage_OnClick(object Sender, RoutedEventArgs E)
        {
            try
            {
                var img = ServiceProvider.WebCamProvider.Capture();

                if (img != null)
                    ServiceProvider.MainViewModel?.SaveScreenShot(img);
            }
            catch { }
        }
    }
}