using Captura.Audio;
using Captura.Models;
using System;
using Captura.Video;
using Captura.Webcam;
using Captura.Windows.Gdi;
using Captura.Windows.MediaFoundation;

namespace Captura.Windows
{
    public static class WindowsModule
    {
        public static void Load(IBinder Binder)
        {
            if (Windows8OrAbove)
            {
                MfManager.Startup();
                Binder.BindAsInterfaceAndClass<IVideoWriterProvider, MfWriterProvider>();
            }

            Binder.BindSingleton<WindowsSettings>();
            Binder.Bind<IPlatformServices, WindowsPlatformServices>();
            Binder.Bind<IDialogService, DialogService>();
            Binder.Bind<IClipboardService, ClipboardService>();
            Binder.Bind<IImagingSystem, DrawingImagingSystem>();
            Binder.Bind<IWebCamProvider, WebcamProvider>();

            foreach (var audioItem in MfAudioItem.Items)
            {
                Binder.Bind<IAudioWriterItem>(() => audioItem);
            }
        }

        public static void Unload()
        {
            if (Windows8OrAbove)
            {
                MfManager.Shutdown();
            }
        }

        public static bool Windows8OrAbove
        {
            get
            {
                // All versions above Windows 8 give the same version number
                var version = new Version(6, 2, 9200, 0);

                return Environment.OSVersion.Platform == PlatformID.Win32NT &&
                       Environment.OSVersion.Version >= version;
            }
        }

        public static bool ShouldUseGdi
        {
            get
            {
                var settings = ServiceProvider.Get<WindowsSettings>();

                return !Windows8OrAbove || settings.UseGdi;
            }
        }
    }
}