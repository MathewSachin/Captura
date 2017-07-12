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

        public bool ShowYesNo(string Message, string Title)
        {
            return Application.Current.Dispatcher.Invoke(() =>
            {
                return MessageBox.Show(Message, Title, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;
            });
        }
    }
}
