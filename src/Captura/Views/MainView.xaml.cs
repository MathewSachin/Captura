using System.Windows;

namespace Captura
{
    public partial class MainView
    {
        void Preview_Click(object Sender, RoutedEventArgs E)
        {
            WebCamWindow.Instance.Show();
        }
    }
}