using Screna;
using System;
using System.Drawing;

namespace Captura.Models
{
    public class MousePointerOverlay : IOverlay
    {
        readonly MouseOverlaySettings _settings;
        
        public MousePointerOverlay(MouseOverlaySettings Settings)
        {
            _settings = Settings;
        }
        
        /// <summary>
        /// Draws overlay.
        /// </summary>
        public void Draw(IBitmapEditor Editor, Func<Point, Point> Transform = null)
        {
            if (!_settings.Display)
                return;

            var clickRadius = _settings.Radius;

            var curPos = MouseCursor.CursorPosition;

            if (Transform != null)
                curPos = Transform(curPos);

            var d = clickRadius * 2;

            var x = curPos.X - clickRadius;
            var y = curPos.Y - clickRadius;

            Editor.FillEllipse(_settings.Color, new RectangleF(x, y, d, d));

            var border = _settings.BorderThickness;

            if (border > 0)
            {
                x -= border / 2;
                y -= border / 2;
                d += border;

                Editor.DrawEllipse(new Pen(_settings.BorderColor, border), new RectangleF(x, y, d, d));
            }
        }

        public void Dispose() { }
    }
}
