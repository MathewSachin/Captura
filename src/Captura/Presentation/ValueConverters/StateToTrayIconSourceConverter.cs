using Captura.Models;
using System;
using System.Globalization;

namespace Captura
{
    public class StateToTrayIconSourceConverter : OneWayConverter
    {        
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((RecorderState)value)
            {
                case RecorderState.Recording:
                    return "record.ico";

                case RecorderState.Paused:
                    return "pause.ico";

                default:
                    return "Captura.ico";
            }
        }
    }
}