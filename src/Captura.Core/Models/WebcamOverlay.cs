using System;
using System.Drawing;
using Screna;

namespace Captura.Models
{
    public class WebcamOverlay : IOverlay
    {
        public void Dispose() { }

        public void Draw(Graphics g, Func<Point, Point> PointTransform = null)
        {
            var webcam = ServiceProvider.WebCamProvider;

            if (webcam.SelectedCam != webcam.AvailableCams[0])
            {
                var img = webcam.Capture();

                if (img == null)
                    return;

                var point = new Point((int)g.VisibleClipBounds.Width - img.Width - 10, (int)g.VisibleClipBounds.Height - img.Height - 10);

                using (img)
                {
                    try { g.DrawImage(img, point); }
                    catch { }
                }
            }
        }
    }
}