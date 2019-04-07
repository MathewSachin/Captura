using System;
using System.Drawing;
using System.Windows.Forms;

namespace Captura.Models
{
    class MouseClickStep : IRecordStep
    {
        readonly MouseClickSettings _settings;
        public MouseEventArgs Args { get; private set; }

        public DateTime Timestamp { get; }

        // Default Double-click time on Windows is 500ms
        const int DoubleClickDelta = 500;

        public MouseClickStep(MouseClickSettings Settings,
            MouseEventArgs Args)
        {
            _settings = Settings;
            this.Args = Args;

            Timestamp = DateTime.Now;
        }

        public bool Merge(IRecordStep NextStep)
        {
            if (Args.Clicks == 1 && NextStep is MouseClickStep nextStep)
            {
                var delta = (nextStep.Timestamp - Timestamp).TotalMilliseconds;

                if (nextStep.Args.Clicks == 2 && delta < DoubleClickDelta)
                {
                    Args = nextStep.Args;

                    return true;
                }
            }

            return false;
        }

        public void Draw(IEditableFrame Editor, Func<Point, Point> PointTransform = null)
        {
            var curPos = Args.Location;

            if (PointTransform != null)
                curPos = PointTransform(curPos);

            float clickRadius = _settings.Radius;

            var d = clickRadius * 2;

            var x = curPos.X - clickRadius;
            var y = curPos.Y - clickRadius;

            var color = GetClickCircleColor();

            Editor.FillEllipse(color, new RectangleF(x, y, d, d));

            var border = _settings.BorderThickness;

            if (border > 0)
            {
                x -= border / 2f;
                y -= border / 2f;
                d += border;

                var borderColor = _settings.BorderColor;

                Editor.DrawEllipse(borderColor, border, new RectangleF(x, y, d, d));
            }

            if (Args.Clicks > 1)
            {
                var font = Editor.GetFont("Arial", 15);
                Editor.DrawString(Args.Clicks.ToString(), font, Color.Black, new RectangleF(x + 10, y + 10, d, d));
            }
        }

        Color GetClickCircleColor()
        {
            if (Args.Button.HasFlag(MouseButtons.Right))
            {
                return _settings.RightClickColor;
            }

            if (Args.Button.HasFlag(MouseButtons.Middle))
            {
                return _settings.MiddleClickColor;
            }

            return _settings.Color;
        }
    }
}