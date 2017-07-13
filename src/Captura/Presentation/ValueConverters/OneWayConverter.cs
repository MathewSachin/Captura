using System;
using System.Globalization;
using System.Windows.Data;

namespace Captura
{
    public abstract class OneWayConverter : IValueConverter
    {
        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
