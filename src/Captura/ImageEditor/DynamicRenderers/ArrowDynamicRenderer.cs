using System;
using System.Linq;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Input.StylusPlugIns;
using System.Windows.Media;

namespace Captura
{
    public class ArrowDynamicRenderer : DynamicRenderer, IDynamicRenderer
    {
        bool _isManipulating;

        Point _firstPoint;

        public ArrowDynamicRenderer()
        {
            _firstPoint = new Point(double.NegativeInfinity, double.NegativeInfinity);
        }

        protected override void OnStylusDown(RawStylusInput RawStylusInput)
        {
            _firstPoint = RawStylusInput.GetStylusPoints().First().ToPoint();
            base.OnStylusDown(RawStylusInput);
        }

        const double ArrowAngle = Math.PI / 180 * 150;
        const int ArrowLength = 10;

        public static void GetArrowPoints(Point Start, Point End, out Point P1, out Point P2)
        {
            var theta = Math.Atan2(End.Y - Start.Y, End.X - Start.X);

            P1 = new Point(End.X + ArrowLength * Math.Cos(theta + ArrowAngle),
                End.Y + ArrowLength * Math.Sin(theta + ArrowAngle));

            P2 = new Point(End.X + ArrowLength * Math.Cos(theta - ArrowAngle),
                End.Y + ArrowLength * Math.Sin(theta - ArrowAngle));
        }

        static void Draw(DrawingContext DrawingContext, Point Start, Point End, Pen Pen)
        {
            LineDynamicRenderer.Prepare(ref Start, ref End);

            DrawingContext.DrawLine(Pen, Start, End);

            GetArrowPoints(Start, End, out var p1, out var p2);

            DrawingContext.DrawLine(Pen, End, p1);
            DrawingContext.DrawLine(Pen, End, p2);
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

            Draw(DrawingContext, _firstPoint, StylusPoints.First().ToPoint(), new Pen(FillBrush, DrawingAttributes.Width));
        }

        protected override void OnStylusUp(RawStylusInput RawStylusInput)
        {
            _firstPoint = new Point(double.NegativeInfinity, double.NegativeInfinity);
            base.OnStylusUp(RawStylusInput);
        }

        public Stroke GetStroke(StylusPointCollection StylusPoints, DrawingAttributes DrawingAttribs)
        {
            return new ArrowStroke(StylusPoints, DrawingAttribs);
        }
    }
}
