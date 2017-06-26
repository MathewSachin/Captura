using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Data;
using WpfColor = System.Windows.Media.Color;

namespace Captura
{
    public class DrawingToWpfColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color c)
                return WpfColor.FromArgb(c.A, c.R, c.G, c.B);

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case string s:
                    return ColorTranslator.FromHtml(s);

                case WpfColor c:
                    return Color.FromArgb(c.A, c.R, c.G, c.B);

                default:
                    return Binding.DoNothing;
            }
        }
    }
}