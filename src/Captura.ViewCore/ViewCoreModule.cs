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

            Binder.BindSingleton<CustomOverlaysViewModel>();
            Binder.BindSingleton<CustomImageOverlaysViewModel>();
            Binder.BindSingleton<CensorOverlaysViewModel>();
        }
    }
}