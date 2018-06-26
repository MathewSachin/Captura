using System;
using System.Globalization;
using System.Windows;
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

        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            if (Value is bool b)
            {
                if (TargetType == typeof(Visibility))
                    return b ? Visibility.Collapsed : Visibility.Visible;

                return !b;
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            return DoConvert(Value);
        }
    }
}