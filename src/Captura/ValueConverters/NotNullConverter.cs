using System;
using System.Collections;
using System.Globalization;
using System.Windows;

namespace Captura
{
    public class NotNullConverter : OneWayConverter
    {
        public override object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            var b = Value != null;

            switch (Value)
            {
                case ICollection collection:
                    b = collection.Count != 0;
                    break;

                case string str:
                    b = !string.IsNullOrWhiteSpace(str);
                    break;

                case int i:
                    b = i != 0;
                    break;

                case double d:
                    b = Math.Abs(d) > 0.01;
                    break;
            }

            if ((Parameter is bool inverse || Parameter is string s && bool.TryParse(s, out inverse)) && inverse)
                b = !b;

            if (TargetType == typeof(Visibility))
                return b ? Visibility.Visible : Visibility.Collapsed;

            return b;
        }
    }
}