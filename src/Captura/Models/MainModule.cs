using System;
using Captura.Models;
using Hardcodet.Wpf.TaskbarNotification;
using Ninject.Modules;

namespace Captura
{
    public class MainModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IMessageProvider>().To<MessageProvider>().InSingletonScope();
            Bind<IRegionProvider>().To<RegionSelector>().InSingletonScope();

            Bind<Func<TaskbarIcon>>().ToConstant<Func<TaskbarIcon>>(() => MainWindow.Instance.SystemTray);

            Bind<ISystemTray>().To<SystemTray>().InSingletonScope();
            Bind<IWebCamProvider>().To<WebCamProvider>().InSingletonScope();
            Bind<IMainWindow>().ToMethod(M => new MainWindowProvider(() => MainWindow.Instance));
        }
    }
}