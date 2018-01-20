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
            Bind<ISystemTray>().ToMethod(M => new SystemTray(() => MainWindow.Instance.SystemTray));
            Bind<IWebCamProvider>().To<WebCamProvider>().InSingletonScope();
            Bind<IMainWindow>().ToMethod(M => new MainWindowProvider(() => MainWindow.Instance));
        }
    }
}