using Captura.Models;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Captura
{
    public class StateToRecordButtonGeometryConverter : IValueConverter
    {
        readonly Geometry _recordGeometry = new EllipseGeometry(new Point(), 50, 50),
            _stopGeometry = new RectangleGeometry(new Rect(new Point(), new Size(50, 50)));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (RecorderState)value == RecorderState.NotRecording ? _recordGeometry : _stopGeometry;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}