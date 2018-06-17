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

        public static void Prepare(ref Point Start, ref Point End, out double Width, out double Height)
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

            Width = End.X - Start.X;
            Height = End.Y - Start.Y;

            if (Width <= 0)
                Width = 1;

            if (Height <= 0)
                Height = 1;

            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
            {
                Width = Height = Math.Min(Width, Height);
            }
        }

        public static void Draw(DrawingContext DrawingContext, Point Start, Point End, Pen Pen)
        {
            Prepare(ref Start, ref End, out var w, out var h);
            
            var r = new Rect(Start, new Size(w, h));

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
