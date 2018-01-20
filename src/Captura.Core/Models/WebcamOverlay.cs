using System;
using System.Drawing;
using Screna;
using System.Drawing.Imaging;

namespace Captura.Models
{
    public class WebcamOverlay : IOverlay
    {
        readonly IWebCamProvider _webCamProvider;

        public WebcamOverlay(IWebCamProvider WebCamProvider)
        {
            _webCamProvider = WebCamProvider;
        }

        public void Dispose() { }

        static Point GetPosition(Size Bounds, Size ImageSize)
        {
            var point = new Point(Settings.Instance.WebcamOverlay.X, Settings.Instance.WebcamOverlay.Y);

            switch (Settings.Instance.WebcamOverlay.HorizontalAlignment)
            {
                case Alignment.Center:
                    point.X = Bounds.Width / 2 - ImageSize.Width / 2 + point.X;
                    break;

                case Alignment.End:
                    point.X = Bounds.Width - ImageSize.Width - point.X;
                    break;
            }

            switch (Settings.Instance.WebcamOverlay.VerticalAlignment)
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
            // No Webcam
            if (_webCamProvider.AvailableCams.Count < 1 || _webCamProvider.SelectedCam == _webCamProvider.AvailableCams[0])
                return;

            var img = _webCamProvider.Capture();

            if (img == null)
                return;

            if (Settings.Instance.WebcamOverlay.Resize)
                img = img.Resize(new Size(Settings.Instance.WebcamOverlay.ResizeWidth, Settings.Instance.WebcamOverlay.ResizeHeight), false);

            using (img)
            {
                try
                {
                    var point = GetPosition(new Size((int)g.VisibleClipBounds.Width, (int)g.VisibleClipBounds.Height), img.Size);

                    if (Settings.Instance.WebcamOverlay.Opacity < 100)
                    {
                        var colormatrix = new ColorMatrix
                        {
                            Matrix33 = Settings.Instance.WebcamOverlay.Opacity / 100.0f
                        };

                        var imgAttribute = new ImageAttributes();
                        imgAttribute.SetColorMatrix(colormatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                        g.DrawImage(img, new Rectangle(point, img.Size), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imgAttribute);
                    }
                    else g.DrawImage(img, point);
                }
                catch { }
            }
        }
    }
}