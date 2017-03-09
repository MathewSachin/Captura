using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Captura
{
    public class StateToTaskbarOverlayConverter : IValueConverter
    {
        readonly DrawingImage _recordingOverlay = ToDrawingImage(new EllipseGeometry(new Point(), 25, 25), 175, 10),
            _pausedOverlay = ToDrawingImage(Geometry.Parse("M14,19H18V5H14M6,19H10V5H6V19Z"), 230, 0.6);

        static DrawingImage ToDrawingImage(Geometry G, byte Alpha, double StrokeWidth)
        {
            return new DrawingImage(new GeometryDrawing(new SolidColorBrush(Color.FromArgb(Alpha, 255, 0, 0)), new Pen(Brushes.White, StrokeWidth), G));
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch(value)
            {
                case RecorderState.Recording:
                    return _recordingOverlay;

                case RecorderState.Paused:
                    return _pausedOverlay;

                default:
                    return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}