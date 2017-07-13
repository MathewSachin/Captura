using System.Windows;

namespace Captura
{
    public partial class MainConfigView
    {
        void OpenMouseKeyHook(object sender, RoutedEventArgs e)
        {
            MouseKeyHookView.Instance.ShowAndFocus();
        }
    }
}