using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;

namespace Captura
{
    static class SystemTrayManager
    {
        static Action _balloonAction;
        static NotifyIcon _systemTray;

        public static void Init()
        {
            _systemTray = new NotifyIcon
            {
                Icon = Icon.ExtractAssociatedIcon("Captura.exe"),
                Visible = true,
                ContextMenu = new ContextMenu()
            };

            _systemTray.DoubleClick += (s, e) =>
            {
                App.Current.MainWindow.Show();
                App.Current.MainWindow.WindowState = WindowState.Normal;
                App.Current.MainWindow.Focus();
            };

            _systemTray.BalloonTipClicked += (s, e) => _balloonAction?.Invoke();

            _systemTray.ContextMenu.MenuItems.Add("Start/Stop Recording", (s, e) => MainViewModel.Instance.RecordCommand.ExecuteIfCan());

            _systemTray.ContextMenu.MenuItems.Add("Pause/Resume Recording", (s, e) => MainViewModel.Instance.PauseCommand.ExecuteIfCan());

            _systemTray.ContextMenu.MenuItems.Add("Take ScreenShot", (s, e) => MainViewModel.Instance.ScreenShotCommand.ExecuteIfCan());

            var separator = new MenuItem { BarBreak = true };

            _systemTray.ContextMenu.MenuItems.Add(separator);

            _systemTray.ContextMenu.MenuItems.Add("Exit", (s, e) =>
            {
                _systemTray.Visible = false;
                App.Current.Shutdown();
            });
        }
        
        public static void ShowNotification(string Title, string Text, int Duration, Action ClickAction)
        {
            _balloonAction = ClickAction;

            _systemTray.ShowBalloonTip(Duration, Title, Text, ToolTipIcon.None);
        }

        public static void Dispose()
        {
            _systemTray.Visible = false;
        }
    }
}
