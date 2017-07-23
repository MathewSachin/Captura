using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Windows.Controls.Primitives;

namespace Captura.Models
{
    class SystemTray : ISystemTray
    {
        readonly TaskbarIcon _trayIcon;

        public SystemTray(TaskbarIcon TaskbarIcon)
        {
            _trayIcon = TaskbarIcon;
        }

        public void HideNotification()
        {
            _trayIcon.CloseBalloon();
        }

        public void ShowScreenShotNotification(string FilePath)
        {
            if (!Settings.Instance.TrayNotify)
                return;

            var popup = new ScreenShotBalloon(FilePath);
            
            _trayIcon.ShowCustomBalloon(popup, PopupAnimation.Scroll, Settings.Instance.ScreenShotNotifyTimeout);
        }

        public void ShowTextNotification(string Text, int Duration, Action OnClick)
        {
            if (!Settings.Instance.TrayNotify)
                return;

            var balloon = new TextBalloon(Text, OnClick);

            _trayIcon.ShowCustomBalloon(balloon, PopupAnimation.Scroll, Duration);
        }
    }
}
