using Captura.Models;
using Screna;

namespace Captura
{
    public static class WindowsModule
    {
        public static void Load(IBinder Binder)
        {
            Binder.Bind<IPlatformServices, WindowsPlatformServices>();
            Binder.Bind<IDialogService, DialogService>();
            Binder.Bind<IClipboardService, ClipboardService>();
            Binder.Bind<IImagingSystem, DrawingImagingSystem>();
            Binder.Bind<IWebCamProvider, WebcamProvider>();
        }
    }
}