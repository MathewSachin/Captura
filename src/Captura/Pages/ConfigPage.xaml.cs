using System.Windows;
using Captura.Views;

namespace Captura
{
    public partial class ConfigPage
    {
        void OpenOverlayManager(object Sender, RoutedEventArgs E)
        {
            OverlayWindow.ShowInstance();
        }

        void OpenHotkeyManager(object Sender, RoutedEventArgs E)
        {
            HotkeysWindow.ShowInstance();
        }

        void OpenFileNameFormatter(object Sender, RoutedEventArgs E)
        {
            new FileNameFormatWindow().ShowDialog();
        }

        void OpenSounds(object Sender, RoutedEventArgs E)
        {
            new SoundsWindow().ShowDialog();
        }

        void OpenTrayIconConfig(object Sender, RoutedEventArgs E)
        {
            new TrayConfigWindow().ShowDialog();
        }
    }
}
