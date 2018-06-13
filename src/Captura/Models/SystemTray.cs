using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Captura.Models
{
    class SystemTray : ISystemTray
    {
        bool _first = true;

        /// <summary>
        /// Using a Function ensures that the <see cref="TaskbarIcon"/> object is used only after it is initialised.
        /// </summary>
        readonly Func<TaskbarIcon> _trayIcon;
        readonly Settings _settings;

        readonly PopupContainer _popupContainer = new PopupContainer();

        public SystemTray(Func<TaskbarIcon> TaskbarIcon, Settings Settings)
        {
            _trayIcon = TaskbarIcon;
            _settings = Settings;
        }

        public void HideNotification()
        {
            _popupContainer.Visibility = Visibility.Collapsed;
        }

        void Show()
        {
            var trayIcon = _trayIcon.Invoke();

            if (trayIcon != null && _first)
            {
                trayIcon.ShowCustomBalloon(_popupContainer, PopupAnimation.Scroll, null);

                _first = false;
            }

            _popupContainer.Visibility = Visibility.Visible;
        }

        public void ShowScreenShotNotification(string FilePath)
        {
            if (!_settings.UI.TrayNotify)
                return;

            _popupContainer.Add(new ScreenShotBalloon(FilePath));

            Show();
        }

        public void ShowTextNotification(string Text, int Duration, Action OnClick)
        {
            if (!_settings.UI.TrayNotify)
                return;

            _popupContainer.Add(new TextBalloon(Text, OnClick));

            Show();
        }
    }
}
