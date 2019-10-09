using System;
using System.Windows;

namespace Captura.Views
{
    public partial class ErrorWindow
    {
        readonly Exception _exception;
        readonly string _message;

        public ErrorWindow(Exception Exception, string Message = null)
        {
            _exception = Exception;
            _message = Message;
            InitializeComponent();

            if (DataContext is ExceptionViewModel vm)
            {
                vm.Init(Exception, Message);
            }
        }

        void ViewDetails_OnClick(object Sender, RoutedEventArgs E)
        {
            new ExceptionWindow(_exception, _message).ShowAndFocus();

            Close();
        }

        void OpenFFmpegLog(object Sender, RoutedEventArgs E)
        {
            SettingsWindow.ShowFFmpegLogs();
        }
    }
}
