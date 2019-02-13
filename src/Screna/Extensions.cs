using System.Drawing;
using Captura;

namespace Screna
{
    /// <summary>
    /// Collection of utility methods.
    /// </summary>
    public static class Extensions
    {
        public static Rectangle Even(this Rectangle Rect)
        {
            if (Rect.Width % 2 == 1)
                --Rect.Width;

            if (Rect.Height % 2 == 1)
                --Rect.Height;

            return Rect;
        }

        public static void WriteToClipboard(this string S)
        {
            if (S == null)
                return;

            var clipboard = ServiceProvider.Get<IClipboardService>();

            clipboard.SetText(S);
        }
    }
}