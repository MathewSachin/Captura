using System.Windows;

namespace Captura
{
    public partial class MainPage
    {
        void Preview_Click(object Sender, RoutedEventArgs E)
        {
            WebCamWindow.Instance.ShowAndFocus();
        }
        
        void OpenCanvas(object Sender, RoutedEventArgs E)
        {
            new ImageEditorWindow().ShowAndFocus();
        }
    }
}