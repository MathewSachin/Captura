using Captura.ViewModels;

namespace Captura
{
    /// <summary>
    /// Used as a Static Resource to inject ViewModels into UI.
    /// </summary>
    public class NinjectServiceLocator
    {
        static NinjectServiceLocator()
        {
            ServiceProvider.LoadModule(new MainModule());
        }
        
        public MainViewModel MainViewModel => ServiceProvider.Get<MainViewModel>();

        public AboutViewModel AboutViewModel => ServiceProvider.Get<AboutViewModel>();

        public FFMpegDownloadViewModel FFMpegDownloadViewModel => ServiceProvider.Get<FFMpegDownloadViewModel>();

        public ProxySettingsViewModel ProxySettingsViewModel => ServiceProvider.Get<ProxySettingsViewModel>();

        public LicensesViewModel LicensesViewModel => ServiceProvider.Get<LicensesViewModel>();
    }
}