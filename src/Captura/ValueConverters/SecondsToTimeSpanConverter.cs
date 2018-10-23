using System;
using System.Windows.Data;
using System.Globalization;

namespace Captura
{
    public class SecondsToTimeSpanConverter : IValueConverter
    {
        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            if (Value is int seconds)
                return TimeSpan.FromSeconds(seconds);

            return Binding.DoNothing;
        }

        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            if (Value is TimeSpan t)
                return t.TotalSeconds;

            return Binding.DoNothing;
        }
    }
}
