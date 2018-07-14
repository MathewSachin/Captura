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
            Binder.BindSingleton<ScreenShotViewModel>();

            Binder.BindSingleton<CustomOverlaysViewModel>();
            Binder.BindSingleton<CustomImageOverlaysViewModel>();
            Binder.BindSingleton<CensorOverlaysViewModel>();

            // Settings
            Binder.BindSingleton<Settings>();
            Binder.Bind(() => ServiceProvider.Get<Settings>().Audio);
            Binder.Bind(() => ServiceProvider.Get<Settings>().FFmpeg);
            Binder.Bind(() => ServiceProvider.Get<Settings>().Gif);
            Binder.Bind(() => ServiceProvider.Get<Settings>().Proxy);

            // Localization
            Binder.Bind(() => LanguageManager.Instance);

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
            Binder.BindSingleton<ScreenPickerItem>();
            Binder.BindSingleton<FullScreenItem>();

            // Video Source Providers
            Binder.Bind<IVideoSourceProvider, ScreenSourceProvider>();
            Binder.Bind<IVideoSourceProvider, RegionSourceProvider>();
            Binder.Bind<IVideoSourceProvider, WindowSourceProvider>();
            Binder.Bind<IVideoSourceProvider, DeskDuplSourceProvider>();
            Binder.Bind<IVideoSourceProvider, NoVideoSourceProvider>();

            // Folder Browser Dialog
            Binder.Bind<IDialogService, DialogService>();

            // Clipboard
            Binder.Bind<IClipboardService, ClipboardService>();

            // FFmpeg Log
            Binder.BindSingleton<FFmpegLog>();

            // Check if Bass is available
            if (BassAudioSource.Available)
            {
                Binder.Bind<AudioSource, BassAudioSource>();
            }
            else Binder.Bind<AudioSource, NoAudioSource>();
        }
    }
}