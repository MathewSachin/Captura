using Captura.Models;
using Captura.ViewModels;
// ReSharper disable MemberCanBeMadeStatic.Global

namespace Captura
{
    /// <summary>
    /// Used as a Static Resource to inject ViewModels into UI.
    /// </summary>
    public class ServiceLocator
    {
        static ServiceLocator()
        {
            ServiceProvider.LoadModule(new MainModule());
        }
        
        public MainViewModel MainViewModel => ServiceProvider.Get<MainViewModel>();

        public ScreenShotViewModel ScreenShotViewModel => ServiceProvider.Get<ScreenShotViewModel>();

        public AboutViewModel AboutViewModel => ServiceProvider.Get<AboutViewModel>();

        public FFmpegDownloadViewModel FFmpegDownloadViewModel => ServiceProvider.Get<FFmpegDownloadViewModel>();

        public FFmpegLog FFmpegLog => ServiceProvider.Get<FFmpegLog>();

        public FFmpegCodecsViewModel FFmpegCodecsViewModel => ServiceProvider.Get<FFmpegCodecsViewModel>();

        public ProxySettingsViewModel ProxySettingsViewModel => ServiceProvider.Get<ProxySettingsViewModel>();

        public LicensesViewModel LicensesViewModel => ServiceProvider.Get<LicensesViewModel>();

        public CrashLogsViewModel CrashLogsViewModel => ServiceProvider.Get<CrashLogsViewModel>();

        public FileNameFormatViewModel FileNameFormatViewModel => ServiceProvider.Get<FileNameFormatViewModel>();

        public SoundsViewModel SoundsViewModel => ServiceProvider.Get<SoundsViewModel>();

        public KeymapViewModel Keymap => ServiceProvider.Get<KeymapViewModel>();

        public EditorWriter EditorWriter => ServiceProvider.Get<EditorWriter>();

        public HotKeyManager HotKeyManager => ServiceProvider.Get<HotKeyManager>();

        public IIconSet Icons => ServiceProvider.Get<IIconSet>();

        public UpdateCheckerViewModel UpdateCheckerViewModel => ServiceProvider.Get<UpdateCheckerViewModel>();
    }
}