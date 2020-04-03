using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Windows.Controls.Primitives;
using Captura.Audio;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    class SystemTray : ISystemTray, IDisposable
    {
        bool _first = true;

        /// <summary>
        /// Using a Function ensures that the <see cref="TaskbarIcon"/> object is used only after it is initialised.
        /// </summary>
        readonly Func<TaskbarIcon> _trayIcon;
        readonly Settings _settings;
        readonly IAudioPlayer _audioPlayer;

        readonly NotificationStack _notificationStack = new NotificationStack();

        public SystemTray(Func<TaskbarIcon> TaskbarIcon, Settings Settings, IAudioPlayer AudioPlayer)
        {
            _trayIcon = TaskbarIcon;
            _settings = Settings;
            _audioPlayer = AudioPlayer;

            _notificationStack.Opacity = 0;
        }

        public void HideNotification()
        {
            _notificationStack.Hide();
        }

        void Show()
        {
            var trayIcon = _trayIcon.Invoke();

            if (trayIcon != null && _first)
            {
                trayIcon.ShowCustomBalloon(_notificationStack, PopupAnimation.None, null);

                _first = false;
            }

            _audioPlayer.Play(SoundKind.Notification);

            _notificationStack.Show();
        }

        public void ShowScreenShotNotification(string FilePath)
        {
            if (!_settings.Tray.ShowNotifications)
                return;

            _notificationStack.Add(new ScreenShotBalloon(FilePath));

            Show();
        }

        public void ShowNotification(INotification Notification)
        {
            if (!_settings.Tray.ShowNotifications)
                return;

            _notificationStack.Add(new NotificationBalloon(Notification));

            Show();
        }

        public void Dispose()
        {
            _trayIcon.Invoke()?.Dispose();
        }
    }
}
