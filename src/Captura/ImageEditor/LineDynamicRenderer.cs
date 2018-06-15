using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Input.StylusPlugIns;
using System.Windows.Media;

namespace Captura
{
    public class LineDynamicRenderer : DynamicRenderer
    {
        bool _isManipulating;

        Point _firstPoint;

        public LineDynamicRenderer()
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

            DrawingContext.DrawLine(new Pen(FillBrush, 2), _firstPoint, StylusPoints.First().ToPoint());
        }

        protected override void OnStylusUp(RawStylusInput RawStylusInput)
        {
            _firstPoint = new Point(double.NegativeInfinity, double.NegativeInfinity);
            base.OnStylusUp(RawStylusInput);
        }
    }
}
