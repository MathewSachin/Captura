using Captura.Properties;
using System.Windows;
using WPFCustomMessageBox;

namespace Captura.Models
{
    public class MessageProvider : IMessageProvider
    {
        public void ShowError(string Message)
        {
            Application.Current.Dispatcher.Invoke(() => CustomMessageBox.ShowOK(Message, Resources.ErrorOccured, Resources.Ok, MessageBoxImage.Error));
        }

        public bool ShowYesNo(string Message, string Title)
        {
            return Application.Current.Dispatcher.Invoke(() => CustomMessageBox.ShowYesNo(Message, Title, Resources.Yes, Resources.No, MessageBoxImage.Warning) == MessageBoxResult.Yes);
        }
    }
}
