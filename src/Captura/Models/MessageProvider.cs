using Captura.Properties;
using System.Windows;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

namespace Captura.Models
{
    public class MessageProvider : IMessageProvider
    {
        public void ShowError(string Message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show(Message, Resources.ErrorOccured, MessageBoxButton.OK, MessageBoxImage.Error);
            });
        }

        public void ShowFFMpegUnavailable()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {                
                var result = MessageBox.Show("FFMpeg was not found on your system.\n\nSelect FFMpeg Folder if you alrady have FFMpeg on your system, else Download FFMpeg.",
                    "FFMpeg Unavailable",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Warning,
                    (Style) Application.Current.FindResource("NoFFMpegBox"));

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        FFMpegService.SelectFFMpegFolder();
                        break;

                    case MessageBoxResult.No:
                        FFMpegService.FFMpegDownloader?.Invoke();
                        break;
                }
            });
        }

        public bool ShowYesNo(string Message, string Title)
        {
            return Application.Current.Dispatcher.Invoke(() =>
            {
                return MessageBox.Show(Message, Title, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;
            });
        }
    }
}
