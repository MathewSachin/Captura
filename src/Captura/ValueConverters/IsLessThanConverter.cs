using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Captura
{
    public class IsLessThanConverter : OneWayConverter
    {
        public override object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            if (double.TryParse(Value?.ToString(), out var left) && double.TryParse(Parameter?.ToString(), out var right))
            {
                var b =  left < right;

                if (TargetType == typeof(Visibility))
                    return b ? Visibility.Visible : Visibility.Collapsed;

                return b;
            }

            return Binding.DoNothing;
        }
    }
}