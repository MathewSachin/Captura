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
            // Use singleton to ensure the same instance is used every time.
            Bind<IMessageProvider>().To<MessageProvider>().InSingletonScope();
            Bind<IRegionProvider>().To<RegionSelector>().InSingletonScope();
            Bind<ISystemTray>().To<SystemTray>().InSingletonScope();
            Bind<IWebCamProvider>().To<WebCamProvider>().InSingletonScope();
            Bind<AboutViewModel>().ToSelf().InSingletonScope();

            // Bind as a Function to ensure the UI objects are referenced only after they have been created.
            Bind<Func<TaskbarIcon>>().ToConstant<Func<TaskbarIcon>>(() => MainWindow.Instance.SystemTray);
            Bind<IMainWindow>().ToMethod(M => new MainWindowProvider(() => MainWindow.Instance)).InSingletonScope();
        }
    }
}