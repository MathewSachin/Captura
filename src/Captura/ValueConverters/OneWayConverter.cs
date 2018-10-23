using System;
using System.Globalization;
using System.Windows.Data;

namespace Captura
{
    public abstract class OneWayConverter : IValueConverter
    {
        public abstract object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture);

        public virtual object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            return Binding.DoNothing;
        }
    }
}
