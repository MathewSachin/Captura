using Captura.Models;
using Captura.ViewModels;

namespace Captura
{
    public class CoreModule : IModule
    {
        public void OnLoad(IBinder Binder)
        {
            // Singleton View Models
            Binder.BindSingleton<MainViewModel>();
            Binder.BindSingleton<VideoViewModel>();
            Binder.BindSingleton<ProxySettingsViewModel>();
            Binder.BindSingleton<LicensesViewModel>();
            Binder.BindSingleton<CrashLogsViewModel>();
            Binder.BindSingleton<FFmpegCodecsViewModel>();

            Binder.BindSingleton<CustomOverlaysViewModel>();
            Binder.BindSingleton<CustomImageOverlaysViewModel>();
            Binder.BindSingleton<CensorOverlaysViewModel>();

            // Settings
            Binder.BindSingleton<Settings>();

            // Localization
            Binder.Bind<LanguageManager>(() => LanguageManager.Instance);

            // Hotkeys
            Binder.BindSingleton<HotKeyManager>();

            // Image Writers
            Binder.BindSingleton<DiskWriter>();
            Binder.BindSingleton<ClipboardWriter>();
            Binder.BindSingleton<ImgurWriter>();

            Binder.Bind<IImageWriterItem>(ServiceProvider.Get<DiskWriter>);
            Binder.Bind<IImageWriterItem>(ServiceProvider.Get<ClipboardWriter>);
            Binder.Bind<IImageWriterItem>(ServiceProvider.Get<ImgurWriter>);

            // Video Writer Providers
            Binder.Bind<IVideoWriterProvider, FFmpegWriterProvider>();
            Binder.Bind<IVideoWriterProvider, GifWriterProvider>();
            Binder.Bind<IVideoWriterProvider, StreamingWriterProvider>();

#if DEBUG
            Binder.Bind<IVideoWriterProvider, DiscardWriterProvider>();
#endif

            // Check if SharpAvi is available
            if (ServiceProvider.FileExists("SharpAvi.dll"))
            {
                Binder.Bind<IVideoWriterProvider, SharpAviWriterProvider>();
            }

            Binder.BindSingleton<WindowPickerItem>();

            // Video Source Providers
            Binder.Bind<IVideoSourceProvider, ScreenSourceProvider>();
            Binder.Bind<IVideoSourceProvider, RegionSourceProvider>();
            Binder.Bind<IVideoSourceProvider, WindowSourceProvider>();
            Binder.Bind<IVideoSourceProvider, DeskDuplSourceProvider>();
            Binder.Bind<IVideoSourceProvider, NoVideoSourceProvider>();

            // Check if Bass is available
            if (BassAudioSource.Available)
            {
                Binder.Bind<AudioSource, BassAudioSource>();
            }
            else Binder.Bind<AudioSource, NoAudioSource>();
        }
    }
}