using Captura.Models;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Captura
{
    public class StateToPauseButtonAngleConverter : OneWayConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is RecorderState state)
                return state == RecorderState.Paused ? 90 : 0;

            return Binding.DoNothing;
        }
    }
}