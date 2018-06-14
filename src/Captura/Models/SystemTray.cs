using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;

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

        readonly NotificationStack _notificationStack = new NotificationStack();

        public SystemTray(Func<TaskbarIcon> TaskbarIcon, Settings Settings)
        {
            _trayIcon = TaskbarIcon;
            _settings = Settings;

            _notificationStack.Opacity = 0;
        }

        public void HideNotification()
        {
            _notificationStack.BeginAnimation(UIElement.OpacityProperty,
                new DoubleAnimation(0, new Duration(TimeSpan.FromMilliseconds(100))));
        }

        public void ShowMessage(string Message)
        {
            if (!_settings.UI.TrayNotify)
                return;

            _notificationStack.Add(new StatusBalloon(Message, false));

            Show();
        }

        public void ShowError(string Error)
        {
            if (!_settings.UI.TrayNotify)
                return;

            _notificationStack.Add(new StatusBalloon(Error, true));

            Show();
        }

        void Show()
        {
            var trayIcon = _trayIcon.Invoke();

            if (trayIcon != null && _first)
            {
                trayIcon.ShowCustomBalloon(_notificationStack, PopupAnimation.Scroll, null);

                _first = false;
            }

            _notificationStack.BeginAnimation(UIElement.OpacityProperty,
                new DoubleAnimation(1, new Duration(TimeSpan.FromMilliseconds(300))));
        }

        public void ShowScreenShotNotification(string FilePath)
        {
            if (!_settings.UI.TrayNotify)
                return;

            _notificationStack.Add(new ScreenShotBalloon(FilePath));

            Show();
        }

        public void ShowTextNotification(string Text, Action OnClick)
        {
            if (!_settings.UI.TrayNotify)
                return;

            _notificationStack.Add(new TextBalloon(Text, OnClick));

            Show();
        }

        public ITrayProgress ShowProgress()
        {
            var vm = new TrayProgressViewModel();

            if (!_settings.UI.TrayNotify)
                return vm;

            _notificationStack.Add(new ProgressBalloon(vm));

            Show();

            return vm;
        }
    }
}
