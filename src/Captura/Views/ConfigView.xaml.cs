using System.Windows;

namespace Captura
{
    public partial class ConfigView
    {
        void OpenExtras(object sender, RoutedEventArgs e)
        {
            MoreOptionsWindow.Instance.ShowAndFocus();
        }
    }
}
