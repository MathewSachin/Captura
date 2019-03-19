namespace Captura.ViewModels
{
    public class ViewCoreModule : IModule
    {
        public void OnLoad(IBinder Binder)
        {
            Binder.BindSingleton<CrashLogsViewModel>();
            Binder.BindSingleton<FileNameFormatViewModel>();
            Binder.BindSingleton<LicensesViewModel>();
            Binder.BindSingleton<ProxySettingsViewModel>();
            Binder.BindSingleton<SoundsViewModel>();
            Binder.BindSingleton<RecentViewModel>();
            Binder.BindSingleton<UpdateCheckerViewModel>();
            Binder.BindSingleton<ScreenShotViewModel>();
            Binder.BindSingleton<RecordingViewModel>();
            Binder.BindSingleton<MainViewModel>();
            Binder.BindSingleton<HotkeysViewModel>();
            Binder.BindSingleton<FFmpegLogViewModel>();
            Binder.BindSingleton<FFmpegCodecsViewModel>();
            Binder.BindSingleton<ViewConditionsModel>();

            Binder.BindSingleton<CustomOverlaysViewModel>();
            Binder.BindSingleton<CustomImageOverlaysViewModel>();
            Binder.BindSingleton<CensorOverlaysViewModel>();
        }

        public void Dispose() { }
    }
}