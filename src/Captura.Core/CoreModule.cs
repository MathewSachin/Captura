using System;
using Captura.Models;
using Captura.NAudio;
using Captura.ViewModels;
using DesktopDuplication;

namespace Captura
{
    public class CoreModule : IModule
    {
        public void OnLoad(IBinder Binder)
        {
            Binder.Bind<IAudioWriterItem, WaveItem>();

            if (WindowsModule.Windows8OrAbove)
            {
                MfManager.Startup();
                Binder.BindAsInterfaceAndClass<IVideoWriterProvider, MfWriterProvider>();
            }

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

            Binder.BindSingleton<HotKeyManager>();

            Binder.Bind<ILocalizationProvider>(() => LanguageManager.Instance);

            WindowsModule.Load(Binder);
        }

        public void Dispose()
        {
            if (WindowsModule.Windows8OrAbove)
            {
                MfManager.Shutdown();
            }
        }

        static void BindImageWriters(IBinder Binder)
        {
            Binder.BindAsInterfaceAndClass<IImageWriterItem, DiskWriter>();
            Binder.BindAsInterfaceAndClass<IImageWriterItem, ClipboardWriter>();
            Binder.BindAsInterfaceAndClass<IImageWriterItem, ImageUploadWriter>();
        }

        static void BindViewModels(IBinder Binder)
        {
            Binder.BindSingleton<TimerModel>();
            Binder.BindSingleton<ScreenShotModel>();
            Binder.BindSingleton<RecordingModel>();
            Binder.BindSingleton<WebcamModel>();
            Binder.BindSingleton<KeymapViewModel>();

            Binder.Bind<IRefreshable>(Binder.Get<WebcamModel>);
        }

        static void BindUpdateChecker(IBinder Binder)
        {
            var version = ServiceProvider.AppVersion;

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

            Binder.Bind<IRefreshable>(Binder.Get<AudioSource>);
        }

        static void BindVideoSourceProviders(IBinder Binder)
        {
            Binder.BindAsInterfaceAndClass<IVideoSourceProvider, NoVideoSourceProvider>();
            Binder.BindAsInterfaceAndClass<IVideoSourceProvider, AroundMouseSourceProvider>();
            Binder.BindAsInterfaceAndClass<IVideoSourceProvider, WebcamSourceProvider>();
            Binder.BindAsInterfaceAndClass<IVideoSourceProvider, FullScreenSourceProvider>();
            Binder.BindAsInterfaceAndClass<IVideoSourceProvider, ScreenSourceProvider>();
            Binder.BindAsInterfaceAndClass<IVideoSourceProvider, WindowSourceProvider>();
            Binder.BindAsInterfaceAndClass<IVideoSourceProvider, RegionSourceProvider>();

            if (WindowsModule.Windows8OrAbove)
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
            Binder.Bind(() => Binder.Get<Settings>().ImageEditor);
            Binder.Bind(() => Binder.Get<Settings>().Audio);
            Binder.Bind(() => Binder.Get<Settings>().Proxy);
            Binder.Bind(() => Binder.Get<Settings>().Sounds);
            Binder.Bind(() => Binder.Get<Settings>().Imgur);
        }
    }
}