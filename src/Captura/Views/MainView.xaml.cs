using System.Windows;

namespace Captura
{
    public partial class MainView
    {
        void OpenMouseKeyHook(object sender, RoutedEventArgs e)
        {
            MouseKeyHookView.Instance.ShowAndFocus();
        }
    }
}