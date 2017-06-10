using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Data;

namespace Captura
{
    public class ColorToHexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color c)
            {
                return $"#{c.A.ToString("X")}{c.R.ToString("X")}{c.G.ToString("X")}{c.B.ToString("X")}";
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s && (s.Length == 7 || s.Length == 9) && s[0] == '#')
            {
                return ColorTranslator.FromHtml(s);
            }

            return Binding.DoNothing;
        }
    }
}