using Captura.Models;
using System;
using System.Globalization;

namespace Captura
{
    public class StateToPauseButtonAngleConverter : OneWayConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (RecorderState)value == RecorderState.Paused ? 90 : 0;
        }
    }
}