using Captura.Models;
using Captura.MouseKeyHook;
using Captura.ViewModels;
using Captura.Webcam;

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

        public WebcamPage WebcamPage => ServiceProvider.Get<WebcamPage>();
        
        public MainViewModel MainViewModel => ServiceProvider.Get<MainViewModel>();

        public RecentViewModel RecentViewModel => ServiceProvider.Get<RecentViewModel>();

        public ScreenShotViewModel ScreenShotViewModel => ServiceProvider.Get<ScreenShotViewModel>();

        public AboutViewModel AboutViewModel => ServiceProvider.Get<AboutViewModel>();

        public RegionSelectorViewModel RegionSelectorViewModel => ServiceProvider.Get<RegionSelectorViewModel>();

        public FFmpegDownloadViewModel FFmpegDownloadViewModel => ServiceProvider.Get<FFmpegDownloadViewModel>();

        public FFmpegLogViewModel FFmpegLog => ServiceProvider.Get<FFmpegLogViewModel>();

        public IFpsManager FpsManager => ServiceProvider.Get<IFpsManager>();

        public FFmpegCodecsViewModel FFmpegCodecsViewModel => ServiceProvider.Get<FFmpegCodecsViewModel>();

        public ProxySettingsViewModel ProxySettingsViewModel => ServiceProvider.Get<ProxySettingsViewModel>();

        public LicensesViewModel LicensesViewModel => ServiceProvider.Get<LicensesViewModel>();

        public CrashLogsViewModel CrashLogsViewModel => ServiceProvider.Get<CrashLogsViewModel>();

        public FileNameFormatViewModel FileNameFormatViewModel => ServiceProvider.Get<FileNameFormatViewModel>();

        public YouTubeUploaderViewModel YouTubeUploaderViewModel => ServiceProvider.Get<YouTubeUploaderViewModel>();

        public SoundsViewModel SoundsViewModel => ServiceProvider.Get<SoundsViewModel>();

        public KeymapViewModel Keymap => ServiceProvider.Get<KeymapViewModel>();

        public EditorWriter EditorWriter => ServiceProvider.Get<EditorWriter>();

        public HotkeysViewModel HotkeysViewModel => ServiceProvider.Get<HotkeysViewModel>();

        public IIconSet Icons => ServiceProvider.Get<IIconSet>();

        public UpdateCheckerViewModel UpdateCheckerViewModel => ServiceProvider.Get<UpdateCheckerViewModel>();

        public CustomImageOverlaysViewModel CustomImageOverlays => ServiceProvider.Get<CustomImageOverlaysViewModel>();

        public CustomOverlaysViewModel CustomOverlays => ServiceProvider.Get<CustomOverlaysViewModel>();

        public CensorOverlaysViewModel CensorOverlays => ServiceProvider.Get<CensorOverlaysViewModel>();

        public ViewConditionsModel ViewConditions => ServiceProvider.Get<ViewConditionsModel>();

        public TimerModel TimerModel => ServiceProvider.Get<TimerModel>();

        public AudioSourceViewModel AudioSource => ServiceProvider.Get<AudioSourceViewModel>();

        public WebcamModel WebcamModel => ServiceProvider.Get<WebcamModel>();

        public VideoWritersViewModel VideoWritersViewModel => ServiceProvider.Get<VideoWritersViewModel>();

        public VideoSourcesViewModel VideoSourcesViewModel => ServiceProvider.Get<VideoSourcesViewModel>();

        public RecordingViewModel RecordingViewModel => ServiceProvider.Get<RecordingViewModel>();
    }
}