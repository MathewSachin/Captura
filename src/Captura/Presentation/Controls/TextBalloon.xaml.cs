using System.Windows;
using System.Windows.Input;
using Hardcodet.Wpf.TaskbarNotification;
using System;

namespace Captura
{
    public partial class TextBalloon
    {
        readonly Action _onClick;
        
        public TextBalloon(string Text, Action OnClick)
        {
            DataContext = Text;

            _onClick = OnClick;

            InitializeComponent();
        }

        void OnClose()
        {
            //the tray icon assigned this attached property to simplify access
            var taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
            taskbarIcon?.CloseBalloon();
        }

        void CloseButton_Click(object Sender, RoutedEventArgs E) => OnClose();

        /// <summary>
        /// If the users hovers over the balloon, we don't close it.
        /// </summary>
        void grid_MouseEnter(object Sender, MouseEventArgs E)
        {
            // the tray icon assigned this attached property to simplify access
            var taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
            taskbarIcon?.ResetBalloonCloseTimer();
        }

        void TextBlock_MouseUp(object Sender, MouseButtonEventArgs E)
        {
            _onClick?.Invoke();

            OnClose();
        }
    }
}