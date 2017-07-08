using Captura.Models;
using System;
using System.Globalization;

namespace Captura
{
    public class NotRecordingConverter : OneWayConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (RecorderState)value == RecorderState.NotRecording;
        }
    }
}