using Captura.Models;

namespace Captura
{
    public class FakesModule : IModule
    {
        public void OnLoad(IBinder Binder)
        {
            // Webcam Provider
            Binder.Bind<IWebCamProvider, CoreWebCamProvider>();

            Binder.Bind<IMessageProvider, FakeMessageProvider>();
            Binder.Bind<IRegionProvider>(() => FakeRegionProvider.Instance);
            Binder.Bind<ISystemTray, FakeSystemTray>();
            Binder.Bind<IMainWindow, FakeWindowProvider>();
            Binder.Bind<IPreviewWindow, FakePreviewWindow>();
            Binder.Bind<IVideoSourcePicker, FakeVideoSourcePicker>();
            Binder.Bind<IAudioPlayer, FakeAudioPlayer>();
        }
    }
}