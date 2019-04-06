using System;
using System.Drawing;
using System.Windows.Forms;

namespace Captura.Models
{
    public class MouseClickStep : IOverlay
    {
        readonly MouseClickSettings _settings;
        readonly MouseEventArgs _args;

        public MouseClickStep(MouseClickSettings Settings,
            MouseEventArgs Args)
        {
            _settings = Settings;
            _args = Args;
        }

        public void Dispose() { }

        public void Draw(IEditableFrame Editor, Func<Point, Point> PointTransform = null)
        {
            var curPos = _args.Location;

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

            if (_args.Clicks > 1)
            {
                var font = Editor.GetFont("Arial", 15);
                Editor.DrawString(_args.Clicks.ToString(), font, Color.Black, new RectangleF(x + 10, y + 10, d, d));
            }
        }

        Color GetClickCircleColor()
        {
            if (_args.Button.HasFlag(MouseButtons.Right))
            {
                return _settings.RightClickColor;
            }

            if (_args.Button.HasFlag(MouseButtons.Middle))
            {
                return _settings.MiddleClickColor;
            }

            return _settings.Color;
        }
    }
}