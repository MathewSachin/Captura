using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Windows.Controls.Primitives;

namespace Captura.Models
{
    class SystemTray : ISystemTray
    {
        readonly Func<TaskbarIcon> _trayIcon;
        readonly Settings _settings;

        public SystemTray(Func<TaskbarIcon> TaskbarIcon, Settings Settings)
        {
            _trayIcon = TaskbarIcon;
            _settings = Settings;
        }

        public void HideNotification()
        {
            _trayIcon.Invoke()?.CloseBalloon();
        }

        public void ShowScreenShotNotification(string FilePath)
        {
            if (!_settings.UI.TrayNotify)
                return;

            var popup = new ScreenShotBalloon(FilePath);
            
            _trayIcon.Invoke()?.ShowCustomBalloon(popup, PopupAnimation.Scroll, _settings.UI.ScreenShotNotifyTimeout);
        }

        public void ShowTextNotification(string Text, int Duration, Action OnClick)
        {
            if (!_settings.UI.TrayNotify)
                return;

            var balloon = new TextBalloon(Text, OnClick);

            _trayIcon.Invoke()?.ShowCustomBalloon(balloon, PopupAnimation.Scroll, Duration);
        }
    }
}
