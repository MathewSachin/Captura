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

        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            return DoConvert(Value);
        }

        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            return DoConvert(Value);
        }
    }
}