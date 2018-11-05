using System.Linq;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Input.StylusPlugIns;
using System.Windows.Media;
using Captura.ImageEditor;

namespace Captura
{
    public class EllipseDynamicRenderer : DynamicRenderer, IDynamicRenderer
    {
        bool _isManipulating;

        Point _firstPoint;

        public EllipseDynamicRenderer()
        {
            _firstPoint = new Point(double.NegativeInfinity, double.NegativeInfinity);
        }

        protected override void OnStylusDown(RawStylusInput RawStylusInput)
        {
            _firstPoint = RawStylusInput.GetStylusPoints().First().ToPoint();
            base.OnStylusDown(RawStylusInput);
        }

        public static void Draw(DrawingContext DrawingContext, Point Start, Point End, Pen Pen)
        {
            RectangleDynamicRenderer.Prepare(ref Start, ref End, out var w, out var h);

            var center = new Point(Start.X + w / 2, Start.Y + h / 2);

            DrawingContext.DrawEllipse(null, Pen, center, w / 2, h / 2);
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

            Draw(DrawingContext,
                _firstPoint,
                StylusPoints.First().ToPoint(),
                new Pen(FillBrush, 2));
        }

        protected override void OnStylusUp(RawStylusInput RawStylusInput)
        {
            _firstPoint = new Point(double.NegativeInfinity, double.NegativeInfinity);
            base.OnStylusUp(RawStylusInput);
        }

        public Stroke GetStroke(StylusPointCollection StylusPoints, DrawingAttributes DrawingAttribs)
        {
            return new EllipseStroke(StylusPoints, DrawingAttribs);
        }
    }
}
