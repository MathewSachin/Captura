using System.Windows;
using System.Windows.Input;
using System;

namespace Captura
{
    public partial class NotificationBalloon : IRemoveRequester
    {
        public INotification Notification { get; }

        public NotificationBalloon(INotification Notification)
        {
            this.Notification = Notification;

            Notification.RemoveRequested += OnClose;

            InitializeComponent();
        }

        void OnClose()
        {
            RemoveRequested?.Invoke();
        }

        public event Action RemoveRequested;

        void CloseButton_Click(object Sender, RoutedEventArgs E) => OnClose();
        
        void TextBlock_MouseUp(object Sender, MouseButtonEventArgs E)
        {
            Notification.RaiseClick();

            OnClose();
        }
    }
}