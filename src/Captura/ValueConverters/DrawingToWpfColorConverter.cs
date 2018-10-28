using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Data;
using WpfColor = System.Windows.Media.Color;

namespace Captura
{
    public class DrawingToWpfColorConverter : IValueConverter
    {
        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            if (Value is Color c)
                return c.ToWpfColor();

            return Binding.DoNothing;
        }

        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            switch (Value)
            {
                case string s:
                    return ColorTranslator.FromHtml(s);

                case WpfColor c:
                    return c.ToString();

                default:
                    return Binding.DoNothing;
            }
        }
    }
}