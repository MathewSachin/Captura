using System.Windows;

namespace Captura
{
    public partial class MainPage
    {
        void OpenCanvas(object Sender, RoutedEventArgs E)
        {
            new ImageEditorWindow().ShowAndFocus();
        }

        void OpenSettings(object Sender, RoutedEventArgs E)
        {
            SettingsWindow.ShowInstance();
        }
    }
}