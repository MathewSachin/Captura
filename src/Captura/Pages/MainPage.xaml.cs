using System.Windows;

namespace Captura
{
    public partial class MainPage
    {
        void OpenCanvas(object Sender, RoutedEventArgs E)
        {
            new ImageEditorWindow().ShowAndFocus();
        }
    }
}