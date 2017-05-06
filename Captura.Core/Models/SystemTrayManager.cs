using System;
using System.Drawing;
using System.Windows.Forms;

namespace Captura
{
    public static class SystemTrayManager
    {
        // Balloon Click Callback
        static Action _balloonAction;
        static NotifyIcon _systemTray;

        public static void Init()
        {
            _systemTray = new NotifyIcon
            {
                // Use the icon from exe file
                Icon = Icon.ExtractAssociatedIcon("Captura.exe"),
                Visible = true,
                ContextMenu = new ContextMenu()
            };

            _systemTray.DoubleClick += (s, e) =>
            {
                ServiceProvider.Get<Action>(ServiceName.Focus).Invoke();
            };

            _systemTray.BalloonTipClicked += (s, e) =>
            {
                try { _balloonAction?.Invoke(); }
                catch
                {
                    // Suppress Errors
                }
            };

            foreach (var service in new[] { ServiceName.Recording, ServiceName.Pause, ServiceName.ScreenShot, ServiceName.ActiveScreenShot, ServiceName.DesktopScreenShot })
                _systemTray.ContextMenu.MenuItems.Add(ServiceProvider.GetDescription(service), (s, e) => ServiceProvider.Get<Action>(service).Invoke());
            
            var separator = new MenuItem { BarBreak = true };

            _systemTray.ContextMenu.MenuItems.Add(separator);

            _systemTray.ContextMenu.MenuItems.Add("Exit", (s, e) =>
            {
                ServiceProvider.Get<Action>(ServiceName.Exit).Invoke();
            });
        }
        
        public static void ShowNotification(string Title, string Text, int Duration, Action ClickAction)
        {
            if (!Settings.Instance.TrayNotify)
                return;

            _balloonAction = ClickAction;

            _systemTray.ShowBalloonTip(Duration, Title, Text, ToolTipIcon.None);
        }

        public static void Dispose()
        {
            // Hide the System Tray
            _systemTray.Visible = false;

            _systemTray.Dispose();
        }
    }
}
