using Captura.Models;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Captura
{
    public class StateToRecordButtonGeometryConverter : OneWayConverter
    {
        readonly Geometry _recordGeometry = new EllipseGeometry(new Point(), 50, 50),
            _stopGeometry = new RectangleGeometry(new Rect(new Point(), new Size(50, 50)));

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is RecorderState state)
                return state == RecorderState.NotRecording ? _recordGeometry : _stopGeometry;

            return Binding.DoNothing;
        }
    }
}