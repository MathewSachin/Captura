using System.Linq;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;

namespace Captura.ImageEditor
{
    public class EllipseStroke : Stroke
    {
        public EllipseStroke(StylusPointCollection StylusPoints, DrawingAttributes DrawingAttributes) : base(StylusPoints, DrawingAttributes) { }

        protected override void DrawCore(DrawingContext DrawingContext, DrawingAttributes DrawingAttribs)
        {
            var start = StylusPoints.First().ToPoint();
            var end = StylusPoints.Last().ToPoint();

            if (end.X < start.X)
            {
                var t = start.X;
                start.X = end.X;
                end.X = t;
            }

            if (end.Y < start.Y)
            {
                var t = start.Y;
                start.Y = end.Y;
                end.Y = t;
            }

            var w = end.X - start.X;
            var h = end.Y - start.Y;

            if (w <= 0)
                w = 1;

            if (h <= 0)
                h = 1;

            var center = new Point(start.X + w / 2, start.Y + h / 2);

            DrawingContext.DrawEllipse(null, new Pen(new SolidColorBrush(DrawingAttribs.Color), DrawingAttribs.Width), center, w / 2, h / 2);
        }
    }
}