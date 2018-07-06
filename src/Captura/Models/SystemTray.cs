using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Windows.Controls.Primitives;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
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
            _notificationStack.Hide();
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
                trayIcon.ShowCustomBalloon(_notificationStack, PopupAnimation.None, null);

                _first = false;
            }

            _notificationStack.Show();
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
