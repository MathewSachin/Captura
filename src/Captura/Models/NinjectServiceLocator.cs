using Captura.ViewModels;
using Ninject;

namespace Captura
{
    public class NinjectServiceLocator
    {
        static NinjectServiceLocator()
        {
            ServiceProvider.Kernel.Load(new MainModule());
        }
        
        public MainViewModel MainViewModel => ServiceProvider.Kernel.Get<MainViewModel>();

        public AboutViewModel AboutViewModel => ServiceProvider.Kernel.Get<AboutViewModel>();

        public FFMpegDownloadViewModel FFMpegDownloadViewModel => ServiceProvider.Kernel.Get<FFMpegDownloadViewModel>();
    }
}