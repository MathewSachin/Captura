using Captura.Models;
using Ninject.Modules;

namespace Captura.Console
{
    public class MainModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IMessageProvider>().To<FakeMessageProvider>().InSingletonScope();
            Bind<IRegionProvider>().To<FakeRegionProvider>().InSingletonScope();
            Bind<ISystemTray>().To<FakeSystemTray>().InSingletonScope();
            Bind<IWebCamProvider>().To<FakeWebCamProvider>().InSingletonScope();
            Bind<IMainWindow>().To<FakeWindowProvider>().InSingletonScope();
        }
    }
}