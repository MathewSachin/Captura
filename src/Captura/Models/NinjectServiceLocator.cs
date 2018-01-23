using Captura.ViewModels;

namespace Captura
{
    public class NinjectServiceLocator
    {
        static NinjectServiceLocator()
        {
            ServiceProvider.LoadModule(new MainModule());
        }
        
        public MainViewModel MainViewModel => ServiceProvider.Get<MainViewModel>();

        public AboutViewModel AboutViewModel => ServiceProvider.Get<AboutViewModel>();

        public FFMpegDownloadViewModel FFMpegDownloadViewModel => ServiceProvider.Get<FFMpegDownloadViewModel>();
    }
}