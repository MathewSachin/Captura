using System;
using Captura.Models;
using Hardcodet.Wpf.TaskbarNotification;

namespace Captura
{
    public class MainModule : IModule
    {
        public void OnLoad(IBinder Binder)
        {
            // Use singleton to ensure the same instance is used every time.
            Binder.Bind<IMessageProvider, MessageProvider>();
            Binder.Bind<IRegionProvider, RegionSelector>();
            Binder.Bind<ISystemTray, SystemTray>();
            Binder.Bind<IPreviewWindow, PreviewWindowService>();
            Binder.Bind<IVideoSourcePicker, VideoSourcePicker>();

            Binder.Bind<IImageWriterItem, EditorWriter>();

            Binder.Bind<IWebCamProvider, WebCamProvider>();
            
            Binder.BindSingleton<AboutViewModel>();

            // Bind as a Function to ensure the UI objects are referenced only after they have been created.
            Binder.Bind<Func<TaskbarIcon>>(() => () => MainWindow.Instance.SystemTray);
            Binder.Bind<IMainWindow>(() => new MainWindowProvider(() => MainWindow.Instance));
        }
    }
}