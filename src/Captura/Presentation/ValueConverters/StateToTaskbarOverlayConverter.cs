using Captura.Models;
using System;
using System.Globalization;

namespace Captura
{
    public class StateToTaskbarOverlayConverter : OneWayConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch(value)
            {
                case RecorderState.Recording:
                    return "record.ico";

                case RecorderState.Paused:
                    return "pause.ico";

                default:
                    return null;
            }
        }
    }
}