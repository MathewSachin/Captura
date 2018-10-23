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
            switch (Value)
            {
                case bool b when TargetType == typeof(Visibility):
                    return b ? Visibility.Collapsed : Visibility.Visible;

                case bool b:
                    return !b;

                case Visibility visibility when TargetType == typeof(Visibility):
                    return visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

                case Visibility visibility:
                    return visibility == Visibility.Collapsed;

                default:
                    return Binding.DoNothing;
            }
        }

        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            return DoConvert(Value);
        }
    }
}