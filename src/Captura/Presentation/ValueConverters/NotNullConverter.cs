using System;
using System.Globalization;

namespace Captura
{
    public class NotNullConverter : OneWayConverter
    {
        public override object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            return Value != null;
        }
    }
}