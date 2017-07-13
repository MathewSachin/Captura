using Captura.Models;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Captura
{
    public class NotRecordingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (RecorderState)value == RecorderState.NotRecording;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}