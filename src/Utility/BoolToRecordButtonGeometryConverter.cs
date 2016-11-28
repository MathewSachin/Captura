using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Captura
{
    public class BoolToRecordButtonGeometryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? (Geometry)new EllipseGeometry(new Point(), 50, 50)
                : new RectangleGeometry(new Rect(new Point(), new Size(50, 50)));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}