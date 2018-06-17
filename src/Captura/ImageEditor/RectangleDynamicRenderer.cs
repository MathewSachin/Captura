using System;
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

        public static void Draw(DrawingContext DrawingContext, Point Start, Point End, Pen Pen)
        {
            if (End.X < Start.X)
            {
                var t = Start.X;
                Start.X = End.X;
                End.X = t;
            }

            if (End.Y < Start.Y)
            {
                var t = Start.Y;
                Start.Y = End.Y;
                End.Y = t;
            }

            var w = End.X - Start.X;
            var h = End.Y - Start.Y;

            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
            {
                w = h = Math.Min(w, h);
            }

            var r = new Rect(Start, new Size(w <= 0 ? 1 : w, h <= 0 ? 1 : h));

            DrawingContext.DrawRectangle(null, Pen, r);
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
    }
}
