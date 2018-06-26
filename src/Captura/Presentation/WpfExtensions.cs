using System.Windows;
using System.Windows.Media;

namespace Captura
{
    public static class WpfExtensions
    {
        public static void ShowAndFocus(this Window W)
        {
            W.Show();

            W.Activate();
        }

        public static Color ParseColor(string S)
        {
            if (ColorConverter.ConvertFromString(S) is Color c)
                return c;

            return Colors.Transparent;
        }
    }
}
