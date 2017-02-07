using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Captura
{
    public class BoolToTaskbarOverlayConverter : IValueConverter
    {
        DrawingImage _taskbarOverlay = new DrawingImage(new GeometryDrawing(new SolidColorBrush(Color.FromArgb(175, 255, 0, 0)), new Pen(Brushes.White, 10),
                new EllipseGeometry(new Point(), 25, 25)));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? null : _taskbarOverlay;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}