using System;
using System.Windows;

namespace Captura.Views
{
    public partial class ExceptionWindow
    {
        public ExceptionWindow(Exception Exception, string Message = null)
        {
            InitializeComponent();

            if (DataContext is ExceptionViewModel vm)
            {
                vm.Init(Exception, Message);
            }
        }

        void Close_OnClick(object Sender, RoutedEventArgs E)
        {
            Close();
        }

        void OpenFFmpegLog(object Sender, RoutedEventArgs E)
        {
            SettingsWindow.ShowFFmpegLogs();
        }
    }
}
