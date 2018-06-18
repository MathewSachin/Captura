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

        public FFmpegDownloadViewModel FFmpegDownloadViewModel => ServiceProvider.Get<FFmpegDownloadViewModel>();

        public ProxySettingsViewModel ProxySettingsViewModel => ServiceProvider.Get<ProxySettingsViewModel>();

        public LicensesViewModel LicensesViewModel => ServiceProvider.Get<LicensesViewModel>();
        public CrashLogsViewModel CrashLogsViewModel => ServiceProvider.Get<CrashLogsViewModel>();

        public FFmpegCodecsViewModel FFmpegCodecsViewModel => ServiceProvider.Get<FFmpegCodecsViewModel>();
    }
}