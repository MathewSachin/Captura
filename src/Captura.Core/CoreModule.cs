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
            BindViewModels(Binder);
            BindSettings(Binder);
            BindImageWriters(Binder);
            BindVideoWriterProviders(Binder);
            BindVideoSourceProviders(Binder);
            BindAudioSource(Binder);
            BindUpdateChecker(Binder);

            // Recent
            Binder.Bind<IRecentList, RecentListRepository>();
            Binder.Bind<IRecentItemSerializer, FileRecentSerializer>();
            Binder.Bind<IRecentItemSerializer, UploadRecentSerializer>();

            Binder.Bind<IDialogService, DialogService>();
            Binder.Bind<IClipboardService, ClipboardService>();
            Binder.Bind<IImageUploader, ImgurUploader>();
            Binder.Bind<IIconSet, MaterialDesignIcons>();
            Binder.Bind<IImgurApiKeys, ApiKeys>();

            Binder.BindSingleton<FullScreenItem>();
            Binder.BindSingleton<FFmpegLog>();
            Binder.BindSingleton<HotKeyManager>();
            Binder.Bind(() => LanguageManager.Instance);
        }

        public void Dispose() { }

        static void BindImageWriters(IBinder Binder)
        {
            BindAsInterfaceAndClass<IImageWriterItem, DiskWriter>(Binder);
            BindAsInterfaceAndClass<IImageWriterItem, ClipboardWriter>(Binder);
            BindAsInterfaceAndClass<IImageWriterItem, ImageUploadWriter>(Binder);
        }

        static void BindViewModels(IBinder Binder)
        {
            Binder.BindSingleton<TimerModel>();
            Binder.BindSingleton<MainModel>();
            Binder.BindSingleton<ScreenShotModel>();
            Binder.BindSingleton<RecordingModel>();
            Binder.BindSingleton<VideoSourcesViewModel>();
            Binder.BindSingleton<VideoWritersViewModel>();
            Binder.BindSingleton<FFmpegCodecsViewModel>();
            Binder.BindSingleton<KeymapViewModel>();
        }

        static void BindUpdateChecker(IBinder Binder)
        {
            var version = Assembly.GetEntryAssembly()?.GetName().Version;

            if (version?.Major == 0)
            {
                Binder.Bind<IUpdateChecker, DevUpdateChecker>();
            }
            else Binder.Bind<IUpdateChecker, UpdateChecker>();
        }

        static void BindAudioSource(IBinder Binder)
        {
            // Check if Bass is available
            if (BassAudioSource.Available)
            {
                Binder.Bind<AudioSource, BassAudioSource>();
            }
            else Binder.Bind<AudioSource, NAudioSource>();
        }

        static void BindVideoSourceProviders(IBinder Binder)
        {
            BindAsInterfaceAndClass<IVideoSourceProvider, NoVideoSourceProvider>(Binder);
            BindAsInterfaceAndClass<IVideoSourceProvider, FullScreenSourceProvider>(Binder);
            BindAsInterfaceAndClass<IVideoSourceProvider, ScreenSourceProvider>(Binder);
            BindAsInterfaceAndClass<IVideoSourceProvider, WindowSourceProvider>(Binder);
            BindAsInterfaceAndClass<IVideoSourceProvider, RegionSourceProvider>(Binder);

            if (Windows8OrAbove)
            {
                BindAsInterfaceAndClass<IVideoSourceProvider, DeskDuplSourceProvider>(Binder);
            }
        }

        static void BindVideoWriterProviders(IBinder Binder)
        {
            BindAsInterfaceAndClass<IVideoWriterProvider, FFmpegWriterProvider>(Binder);
            BindAsInterfaceAndClass<IVideoWriterProvider, GifWriterProvider>(Binder);
            BindAsInterfaceAndClass<IVideoWriterProvider, SharpAviWriterProvider>(Binder);
            BindAsInterfaceAndClass<IVideoWriterProvider, StreamingWriterProvider>(Binder);
            BindAsInterfaceAndClass<IVideoWriterProvider, DiscardWriterProvider>(Binder);
        }

        static void BindSettings(IBinder Binder)
        {
            Binder.BindSingleton<Settings>();
            Binder.Bind(() => ServiceProvider.Get<Settings>().Audio);
            Binder.Bind(() => ServiceProvider.Get<Settings>().FFmpeg);
            Binder.Bind(() => ServiceProvider.Get<Settings>().Gif);
            Binder.Bind(() => ServiceProvider.Get<Settings>().Proxy);
            Binder.Bind(() => ServiceProvider.Get<Settings>().Sounds);
            Binder.Bind(() => ServiceProvider.Get<Settings>().Imgur);
        }

        static bool Windows8OrAbove
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