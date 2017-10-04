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

        void CloseButton_Click(object sender = null, RoutedEventArgs e = null)
        {
            //the tray icon assigned this attached property to simplify access
            var taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
            taskbarIcon?.CloseBalloon();
        }

        /// <summary>
        /// If the users hovers over the balloon, we don't close it.
        /// </summary>
        void grid_MouseEnter(object sender, MouseEventArgs e)
        {
            // the tray icon assigned this attached property to simplify access
            var taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
            taskbarIcon?.ResetBalloonCloseTimer();
        }

        void TextBlock_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _onClick?.Invoke();

            CloseButton_Click();
        }
    }
}