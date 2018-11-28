using System;
using System.Reflection;
using Captura.Models;
using Captura.NAudio;
using Captura.ViewModels;

namespace Captura
{
    public class CoreModule : IModule
    {
        /// <summary>
        /// Binds both as Inteface as Class
        /// </summary>
        static void BindAsInterfaceAndClass<TInterface, TClass>(IBinder Binder) where  TClass : TInterface
        {
            Binder.BindSingleton<TClass>();

            // ReSharper disable once ConvertClosureToMethodGroup
            Binder.Bind<TInterface>(() => ServiceProvider.Get<TClass>());
        }

        public void OnLoad(IBinder Binder)
        {
            Binder.BindSingleton<HotkeyActionRegisterer>();
            Binder.Bind<IIconSet, MaterialDesignIcons>();
            Binder.Bind<IImagurApiKeys, ApiKeys>();

            // Singleton View Models
            Binder.BindSingleton<MainViewModel>();
            Binder.BindSingleton<VideoSourcesViewModel>();
            Binder.BindSingleton<VideoWritersViewModel>();
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
            BindAsInterfaceAndClass<IImageWriterItem, DiskWriter>(Binder);
            BindAsInterfaceAndClass<IImageWriterItem, ClipboardWriter>(Binder);
            BindAsInterfaceAndClass<IImageWriterItem, ImageUploadWriter>(Binder);

            Binder.Bind<IImageUploader, ImgurUploader>();

            // Video Writer Providers
            BindAsInterfaceAndClass<IVideoWriterProvider, FFmpegWriterProvider>(Binder);
            BindAsInterfaceAndClass<IVideoWriterProvider, GifWriterProvider>(Binder);
            BindAsInterfaceAndClass<IVideoWriterProvider, SharpAviWriterProvider>(Binder);
            BindAsInterfaceAndClass<IVideoWriterProvider, StreamingWriterProvider>(Binder);
            BindAsInterfaceAndClass<IVideoWriterProvider, DiscardWriterProvider>(Binder);

            Binder.BindSingleton<FullScreenItem>();

            // Video Source Providers
            BindAsInterfaceAndClass<IVideoSourceProvider, NoVideoSourceProvider>(Binder);
            BindAsInterfaceAndClass<IVideoSourceProvider, FullScreenSourceProvider>(Binder);
            BindAsInterfaceAndClass<IVideoSourceProvider, ScreenSourceProvider>(Binder);
            BindAsInterfaceAndClass<IVideoSourceProvider, WindowSourceProvider>(Binder);
            BindAsInterfaceAndClass<IVideoSourceProvider, RegionSourceProvider>(Binder);

            if (Windows8OrAbove)
            {
                BindAsInterfaceAndClass<IVideoSourceProvider, DeskDuplSourceProvider>(Binder);
            }


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

            // Update Checker
            var version = Assembly.GetEntryAssembly().GetName().Version;

            if (version.Major == 0)
            {
                Binder.Bind<IUpdateChecker, DevUpdateChecker>();
            }
            else Binder.Bind<IUpdateChecker, UpdateChecker>();
        }

        bool Windows8OrAbove
        {
            get
            {
                // All versions above Windows 8 give the same version number
                var version = new Version(6, 2, 9200, 0);

                return Environment.OSVersion.Platform == PlatformID.Win32NT &&
                       Environment.OSVersion.Version >= version;
            }
        }
    }
}