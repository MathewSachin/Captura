using System;
using System.Drawing;
using System.Windows.Forms;

namespace Captura.Models
{
    public class ScrollOverlay : IOverlay
    {
        readonly MouseClickSettings _settings;
        MouseEventArgs _lastArgs;

        public ScrollOverlay(IMouseKeyHook Hook,
            MouseClickSettings Settings)
        {
            _settings = Settings;

            Hook.MouseWheel += (S, E) => _lastArgs = E;
        }

        public void Dispose() { }

        public void Draw(IEditableFrame Editor, Func<Point, Point> PointTransform = null)
        {
            if (!_settings.DisplayScroll)
                return;

            if (_lastArgs is { } args)
            {
                var p = args.Location;
                var r = _settings.Radius;

                var above = new Point(p.X, p.Y + r / 2);
                var below = new Point(p.X, p.Y - r / 2);

                // Scroll down
                if (args.Delta < 0)
                {
                    (above, below) = (below, above);
                }

                Editor.DrawArrow(above, below, _settings.BorderColor, r / 4f);

                _lastArgs = null;
            }
        }
    }
}