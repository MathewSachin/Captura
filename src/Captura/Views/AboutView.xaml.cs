using System.Windows;

namespace Captura
{
    public partial class AboutView
    {
        void ShowAndFocus(Window W)
        {
            W.Show();

            W.Activate();
        }

        void OpenExtras(object sender, RoutedEventArgs e)
        {
            ShowAndFocus(MoreOptionsWindow.Instance);
        }

        void OpenMouseKeyHook(object sender, RoutedEventArgs e)
        {
            ShowAndFocus(MouseKeyHookView.Instance);
        }
    }
}
