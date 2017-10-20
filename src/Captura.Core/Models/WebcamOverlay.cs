﻿using System;
using System.Drawing;
using Screna;

namespace Captura.Models
{
    public class WebcamOverlay : IOverlay
    {
        public void Dispose() { }

        static Point GetPosition(Size Bounds, Size ImageSize)
        {
            var point = new Point(Settings.Instance.Webcam_X, Settings.Instance.Webcam_Y);

            switch (Settings.Instance.Webcam_XAlign)
            {
                case Alignment.Center:
                    point.X = Bounds.Width / 2 - ImageSize.Width / 2 + point.X;
                    break;

                case Alignment.End:
                    point.X = Bounds.Width - ImageSize.Width - point.X;
                    break;
            }

            switch (Settings.Instance.Webcam_YAlign)
            {
                case Alignment.Center:
                    point.Y = Bounds.Height / 2 - ImageSize.Height / 2 + point.Y;
                    break;

                case Alignment.End:
                    point.Y = Bounds.Height - ImageSize.Height - point.Y;
                    break;
            }

            return point;
        }

        public void Draw(Graphics g, Func<Point, Point> PointTransform = null)
        {
            var webcam = ServiceProvider.WebCamProvider;

            if (webcam.SelectedCam != webcam.AvailableCams[0])
            {
                var img = webcam.Capture();

                if (img == null)
                    return;
                
                using (img)
                {
                    try
                    {
                        var point = GetPosition(new Size((int)g.VisibleClipBounds.Width, (int)g.VisibleClipBounds.Height), img.Size);

                        g.DrawImage(img, point);
                    }
                    catch { }
                }
            }
        }
    }
}