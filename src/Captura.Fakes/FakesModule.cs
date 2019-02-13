using Captura.Models;

namespace Captura
{
    public class FakesModule : IModule
    {
        public void OnLoad(IBinder Binder)
        {
            Binder.Bind<IMessageProvider, FakeMessageProvider>();
            Binder.Bind<IRegionProvider>(() => FakeRegionProvider.Instance);
            Binder.Bind<ISystemTray, FakeSystemTray>();
            Binder.Bind<IMainWindow, FakeWindowProvider>();
            Binder.Bind<IPreviewWindow, FakePreviewWindow>();
            Binder.Bind<IVideoSourcePicker>(() => FakeVideoSourcePicker.Instance);
            Binder.Bind<IAudioPlayer, FakeAudioPlayer>();
        }

        public void Dispose() { }
    }
}