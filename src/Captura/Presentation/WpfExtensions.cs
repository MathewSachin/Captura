using System.Windows;

namespace Captura
{
    public static class WpfExtensions
    {
        public static void ShowAndFocus(this Window W)
        {
            W.Show();

            W.Activate();
        }
    }
}
