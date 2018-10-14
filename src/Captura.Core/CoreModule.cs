using Captura.Models;
using Captura.ViewModels;

namespace Captura
{
    public class CoreModule : IModule
    {
        public void OnLoad(IBinder Binder)
        {
            Binder.BindSingleton<HotkeyActionRegisterer>();

            // Singleton View Models
            Binder.BindSingleton<MainViewModel>();
            Binder.BindSingleton<VideoViewModel>();
            Binder.BindSingleton<ProxySettingsViewModel>();
            Binder.BindSingleton<LicensesViewModel>();
            Binder.BindSingleton<CrashLogsViewModel>();
            Binder.BindSingleton<FFmpegCodecsViewModel>();
            Binder.BindSingleton<ScreenShotViewModel>();
            Binder.BindSingleton<RecentViewModel>();
            Binder.BindSingleton<RecordingViewModel>();
            Binder.BindSingleton<FileNameFormatViewModel>();

            Binder.BindSingleton<CustomOverlaysViewModel>();
            Binder.BindSingleton<CustomImageOverlaysViewModel>();
            Binder.BindSingleton<CensorOverlaysViewModel>();

            // Settings
            Binder.BindSingleton<Settings>();
            Binder.Bind(() => ServiceProvider.Get<Settings>().Audio);
            Binder.Bind(() => ServiceProvider.Get<Settings>().FFmpeg);
            Binder.Bind(() => ServiceProvider.Get<Settings>().Gif);
            Binder.Bind(() => ServiceProvider.Get<Settings>().Proxy);
            Binder.Bind(() => ServiceProvider.Get<Settings>().Sounds);

            // Keymap
            Binder.BindSingleton<KeymapViewModel>();

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
            Binder.BindSingleton<FFmpegWriterProvider>();
            Binder.BindSingleton<GifWriterProvider>();
            Binder.BindSingleton<StreamingWriterProvider>();
            Binder.BindSingleton<DiscardWriterProvider>();
            Binder.BindSingleton<SharpAviWriterProvider>();

            Binder.BindSingleton<FullScreenItem>();

            // Video Source Providers
            Binder.BindSingleton<ScreenSourceProvider>();
            Binder.BindSingleton<FullScreenSourceProvider>();
            Binder.BindSingleton<RegionSourceProvider>();
            Binder.BindSingleton<WindowSourceProvider>();
            Binder.BindSingleton<DeskDuplSourceProvider>();
            Binder.BindSingleton<NoVideoSourceProvider>();

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