using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;

namespace Captura
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

        const string RectRegex = @"-?\d+,-?\d+,\d+,\d+";

        public static Rectangle? ConvertToRectangle(this string Value)
        {
            if (Regex.IsMatch(Value, RectRegex))
            {
                var x = Value.Split(',')
                    .Select(int.Parse)
                    .ToArray();

                return new Rectangle(x[0], x[1], x[2], x[3]);
            }

            return null;
        }

        public static string ConvertToString(this Rectangle Rect)
        {
            var x = new[] { Rect.X, Rect.Y, Rect.Width, Rect.Height };

            return string.Join(",", x.Select(M => M.ToString()));
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