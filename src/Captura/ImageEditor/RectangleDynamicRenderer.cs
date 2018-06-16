using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Input.StylusPlugIns;
using System.Windows.Media;

namespace Captura
{
    public class RectangleDynamicRenderer : DynamicRenderer
    {
        bool _isManipulating;

        Point _firstPoint;

        public RectangleDynamicRenderer()
        {
            _firstPoint = new Point(double.NegativeInfinity, double.NegativeInfinity);
        }

        protected override void OnStylusDown(RawStylusInput RawStylusInput)
        {
            _firstPoint = RawStylusInput.GetStylusPoints().First().ToPoint();
            base.OnStylusDown(RawStylusInput);
        }

        protected override void OnDraw(DrawingContext DrawingContext, StylusPointCollection StylusPoints, Geometry Geometry, Brush FillBrush)
        {
            if (!_isManipulating)
            {
                _isManipulating = true;

                var currentStylus = Stylus.CurrentStylusDevice;
                Reset(currentStylus, StylusPoints);
            }

            _isManipulating = false;

            var first = new Point(_firstPoint.X, _firstPoint.Y);
            var last = StylusPoints.First().ToPoint();

            if (last.X < first.X)
            {
                var t = first.X;
                first.X = last.X;
                last.X = t;
            }

            if (last.Y < first.Y)
            {
                var t = first.Y;
                first.Y = last.Y;
                last.Y = t;
            }

            var w = last.X - first.X;
            var h = last.Y - first.Y;

            var r = new Rect(first, new Size(w <= 0 ? 1 : w, h <= 0 ? 1 : h));

            DrawingContext.DrawRectangle(null, new Pen(FillBrush, 2), r);
        }

        protected override void OnStylusUp(RawStylusInput RawStylusInput)
        {
            _firstPoint = new Point(double.NegativeInfinity, double.NegativeInfinity);
            base.OnStylusUp(RawStylusInput);
        }
    }
}
