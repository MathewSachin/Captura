using System.Windows;

namespace Captura
{
    public partial class ConfigPage
    {
        void OpenOverlayManager(object Sender, RoutedEventArgs E)
        {
            OverlayWindow.ShowInstance();
        }

        void OpenSettings(object Sender, RoutedEventArgs E)
        {
            new SettingsWindow().ShowDialog();
        }
    }
}
