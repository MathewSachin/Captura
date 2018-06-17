using System;
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

        public static void Draw(DrawingContext DrawingContext, Point Start, Point End, Pen Pen)
        {
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
            {
                var sdx = End.X - Start.X;
                var sdy = End.Y - Start.Y;

                var dx = Math.Abs(sdx);
                var dy = Math.Abs(sdy);

                if (dx < dy / 2)
                {
                    End.X = Start.X;
                }
                else if (dy < dx / 2)
                {
                    End.Y = Start.Y;
                }
                else
                {
                    var d = Math.Min(dx, dy);

                    End.X = Start.X + Math.Sign(sdx) * d;
                    End.Y = Start.Y + Math.Sign(sdy) * d;
                }
            }

            DrawingContext.DrawLine(Pen, Start, End);
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

            Draw(DrawingContext, _firstPoint, StylusPoints.First().ToPoint(), new Pen(FillBrush, 2));
        }

        protected override void OnStylusUp(RawStylusInput RawStylusInput)
        {
            _firstPoint = new Point(double.NegativeInfinity, double.NegativeInfinity);
            base.OnStylusUp(RawStylusInput);
        }
    }
}
