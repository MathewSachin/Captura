using System.Windows;
using Captura.Models;
using Captura.ViewModels;
using Ninject;

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
                var img = NinjectServiceLocator.Kernel.Get<WebCamProvider>().Capture();
                
                if (img != null)
                    NinjectServiceLocator.Kernel.Get<MainViewModel>().SaveScreenShot(img);
            }
            catch { }
        }
    }
}