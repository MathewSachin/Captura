using System.Windows;
using WebEye.Controls.Wpf;

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

        public WebCameraControl GetWebCamControl() => webCameraControl;

        void CloseButton_Click(object Sender, RoutedEventArgs E) => Close();

        void CaptureImage_OnClick(object Sender, RoutedEventArgs E)
        {
            ServiceProvider.MainViewModel?.SaveScreenShot(webCameraControl.GetCurrentImage());
        }
    }
}