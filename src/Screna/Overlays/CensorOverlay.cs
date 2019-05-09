using System;
using System.Collections.Generic;
using System.Drawing;

namespace Captura
{
    public class CensorOverlay : IOverlay
    {
        readonly IEnumerable<CensorOverlaySettings> _overlaySettings;

        public CensorOverlay(IEnumerable<CensorOverlaySettings> OverlaySettings)
        {
            _overlaySettings = OverlaySettings;
        }

        public void Dispose() { }

        public void Draw(IEditableFrame Editor, Func<Point, Point> PointTransform = null)
        {
            foreach (var overlaySetting in _overlaySettings)
            {
                if (!overlaySetting.Display)
                    continue;

                var x = overlaySetting.GetX(Editor.Width);
                var y = overlaySetting.GetY(Editor.Height);

                Editor.FillRectangle(Color.Black,
                    new RectangleF(
                        (float)x,
                        (float)y,
                        overlaySetting.Width,
                        overlaySetting.Height));
            }
        }
    }
}