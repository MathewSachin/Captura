using System;
using System.Windows.Data;
using System.Globalization;

namespace Captura
{
    public class TimeSpanToSecondsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan t)
                return t.TotalSeconds;

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double seconds)
                return TimeSpan.FromSeconds((int)seconds);

            return Binding.DoNothing;
        }
    }
}
