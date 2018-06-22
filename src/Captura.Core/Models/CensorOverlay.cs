using System;
using System.Collections.Generic;
using System.Drawing;
using Screna;

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

        static float GetLeft(CensorOverlaySettings OverlaySettings, float FullWidth)
        {
            var x = OverlaySettings.X;

            switch (OverlaySettings.HorizontalAlignment)
            {
                case Alignment.Start:
                    return x;

                case Alignment.End:
                    return FullWidth - x - OverlaySettings.Width;

                case Alignment.Center:
                    return FullWidth / 2 + x - OverlaySettings.Width / 2f;

                default:
                    return 0;
            }
        }

        static float GetTop(CensorOverlaySettings OverlaySettings, float FullHeight)
        {
            var y = OverlaySettings.Y;

            switch (OverlaySettings.VerticalAlignment)
            {
                case Alignment.Start:
                    return y;

                case Alignment.End:
                    return FullHeight - y - OverlaySettings.Height;

                case Alignment.Center:
                    return FullHeight / 2 + y - OverlaySettings.Height / 2f;

                default:
                    return 0;
            }
        }

        public void Draw(Graphics G, Func<Point, Point> PointTransform = null)
        {
            foreach (var overlaySetting in _overlaySettings)
            {
                if (!overlaySetting.Display)
                    continue;

                G.FillRectangle(new SolidBrush(Color.Black),
                    GetLeft(overlaySetting, G.VisibleClipBounds.Width),
                    GetTop(overlaySetting, G.VisibleClipBounds.Height),
                    overlaySetting.Width,
                    overlaySetting.Height);
            }
        }
    }
}