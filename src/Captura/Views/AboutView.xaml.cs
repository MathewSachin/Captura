using System.Windows;

namespace Captura
{
    public partial class AboutView
    {
        void OpenExtras(object sender, RoutedEventArgs e)
        {
            MoreOptionsWindow.Instance.Show();
        }

        void OpenMouseKeyHook(object sender, RoutedEventArgs e)
        {
            MouseKeyHookView.Instance.Show();
        }
    }
}
