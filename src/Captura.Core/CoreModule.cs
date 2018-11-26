using Captura.Models;
using Captura.NAudio;
using Captura.ViewModels;

namespace Captura
{
    public class CoreModule : IModule
    {
        public void OnLoad(IBinder Binder)
        {
            Binder.BindSingleton<HotkeyActionRegisterer>();
            Binder.Bind<IIconSet, MaterialDesignIcons>();
            Binder.Bind<IImagurApiKeys, ApiKeys>();

            // Singleton View Models
            Binder.BindSingleton<MainViewModel>();
            Binder.BindSingleton<VideoViewModel>();
            Binder.BindSingleton<FFmpegCodecsViewModel>();
            Binder.BindSingleton<ScreenShotViewModel>();
            Binder.BindSingleton<RecordingViewModel>();

            Binder.Bind<IRecentList, RecentListRepository>();

            // Settings
            Binder.BindSingleton<Settings>();
            Binder.Bind(() => ServiceProvider.Get<Settings>().Audio);
            Binder.Bind(() => ServiceProvider.Get<Settings>().FFmpeg);
            Binder.Bind(() => ServiceProvider.Get<Settings>().Gif);
            Binder.Bind(() => ServiceProvider.Get<Settings>().Proxy);
            Binder.Bind(() => ServiceProvider.Get<Settings>().Sounds);
            Binder.Bind(() => ServiceProvider.Get<Settings>().Imgur);

            // Keymap
            Binder.BindSingleton<KeymapViewModel>();

            // Localization
            Binder.Bind(() => LanguageManager.Instance);

            // Hotkeys
            Binder.BindSingleton<HotKeyManager>();

            // Image Writers
            Binder.BindSingleton<DiskWriter>();
            Binder.BindSingleton<ClipboardWriter>();
            Binder.BindSingleton<ImageUploadWriter>();

            Binder.Bind<IImageUploader, ImgurUploader>();

            Binder.Bind<IImageWriterItem>(ServiceProvider.Get<DiskWriter>);
            Binder.Bind<IImageWriterItem>(ServiceProvider.Get<ClipboardWriter>);
            Binder.Bind<IImageWriterItem>(ServiceProvider.Get<ImageUploadWriter>);

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

            // Recent Serializers
            Binder.Bind<IRecentItemSerializer, FileRecentSerializer>();
            Binder.Bind<IRecentItemSerializer, UploadRecentSerializer>();

            // Check if Bass is available
            if (BassAudioSource.Available)
            {
                Binder.Bind<AudioSource, BassAudioSource>();
            }
            else Binder.Bind<AudioSource, NAudioSource>();
        }
    }
}