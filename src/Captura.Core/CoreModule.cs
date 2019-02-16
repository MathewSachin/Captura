using System;
using System.Reflection;
using Captura.Models;
using Captura.NAudio;
using Captura.ViewModels;

namespace Captura
{
    public class CoreModule : IModule
    {
        public void OnLoad(IBinder Binder)
        {
            Binder.Bind(() => WaveItem.Instance);

            FFmpegModule.Load(Binder);

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

            Binder.Bind<IImageUploader, ImgurUploader>();
            Binder.Bind<IIconSet, MaterialDesignIcons>();
            Binder.Bind<IImgurApiKeys, ApiKeys>();
            Binder.Bind<IYouTubeApiKeys, ApiKeys>();

            Binder.BindSingleton<FullScreenItem>();
            Binder.BindSingleton<HotKeyManager>();
            Binder.Bind(() => LanguageManager.Instance);

            WindowsModule.Load(Binder);
        }

        public void Dispose() { }

        static void BindImageWriters(IBinder Binder)
        {
            Binder.BindAsInterfaceAndClass<IImageWriterItem, DiskWriter>();
            Binder.BindAsInterfaceAndClass<IImageWriterItem, ClipboardWriter>();
            Binder.BindAsInterfaceAndClass<IImageWriterItem, ImageUploadWriter>();
        }

        static void BindViewModels(IBinder Binder)
        {
            Binder.BindSingleton<TimerModel>();
            Binder.BindSingleton<MainModel>();
            Binder.BindSingleton<ScreenShotModel>();
            Binder.BindSingleton<RecordingModel>();
            Binder.BindSingleton<WebcamModel>();
            Binder.BindSingleton<VideoSourcesViewModel>();
            Binder.BindSingleton<VideoWritersViewModel>();
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
            Binder.BindAsInterfaceAndClass<IVideoSourceProvider, NoVideoSourceProvider>();
            Binder.BindAsInterfaceAndClass<IVideoSourceProvider, FullScreenSourceProvider>();
            Binder.BindAsInterfaceAndClass<IVideoSourceProvider, ScreenSourceProvider>();
            Binder.BindAsInterfaceAndClass<IVideoSourceProvider, WindowSourceProvider>();
            Binder.BindAsInterfaceAndClass<IVideoSourceProvider, RegionSourceProvider>();

            if (Windows8OrAbove)
            {
                Binder.BindAsInterfaceAndClass<IVideoSourceProvider, DeskDuplSourceProvider>();
            }
        }

        static void BindVideoWriterProviders(IBinder Binder)
        {
            Binder.BindAsInterfaceAndClass<IVideoWriterProvider, SharpAviWriterProvider>();
            Binder.BindAsInterfaceAndClass<IVideoWriterProvider, DiscardWriterProvider>();
        }

        static void BindSettings(IBinder Binder)
        {
            Binder.BindSingleton<Settings>();
            Binder.Bind(() => ServiceProvider.Get<Settings>().Audio);
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