using System;
using System.Globalization;

namespace Captura
{
    public class NotNullConverter : OneWayConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }
    }
}