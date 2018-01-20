using Captura.Models;
using Ninject.Modules;

namespace Captura
{
    public class MainModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IMessageProvider>().To<MessageProvider>().InSingletonScope();
            Bind<IRegionProvider>().To<RegionSelector>().InSingletonScope();
            Bind<ISystemTray>().To<SystemTray>().InSingletonScope();
            Bind<IWebCamProvider>().To<WebCamProvider>().InSingletonScope();
            Bind<IMainWindow>().To<MainWindowProvider>().InSingletonScope();
        }
    }
}