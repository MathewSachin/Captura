using System;
using System.Globalization;
using System.Windows.Data;

namespace Captura
{
    public class NegatingConverter : IValueConverter
    {
        static object DoConvert(object Value)
        {
            if (Value is bool b)
                return !b;

            return Binding.DoNothing;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DoConvert(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DoConvert(value);
        }
    }
}