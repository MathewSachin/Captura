using System;
using System.Drawing;
using Screna;
using System.Drawing.Imaging;

namespace Captura.Models
{
    public class WebcamOverlay : IOverlay
    {
        readonly IWebCamProvider _webCamProvider;
        readonly Settings _settings;

        public WebcamOverlay(IWebCamProvider WebCamProvider, Settings Settings)
        {
            _webCamProvider = WebCamProvider;
            _settings = Settings;
        }

        public void Dispose() { }

        Point GetPosition(Size Bounds, Size ImageSize)
        {
            var point = new Point(_settings.WebcamOverlay.X, _settings.WebcamOverlay.Y);

            switch (_settings.WebcamOverlay.HorizontalAlignment)
            {
                case Alignment.Center:
                    point.X = Bounds.Width / 2 - ImageSize.Width / 2 + point.X;
                    break;

                case Alignment.End:
                    point.X = Bounds.Width - ImageSize.Width - point.X;
                    break;
            }

            switch (_settings.WebcamOverlay.VerticalAlignment)
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

            if (_settings.WebcamOverlay.Resize)
                img = img.Resize(new Size(_settings.WebcamOverlay.ResizeWidth, _settings.WebcamOverlay.ResizeHeight), false);

            using (img)
            {
                try
                {
                    var point = GetPosition(new Size((int)g.VisibleClipBounds.Width, (int)g.VisibleClipBounds.Height), img.Size);

                    if (_settings.WebcamOverlay.Opacity < 100)
                    {
                        var colormatrix = new ColorMatrix
                        {
                            Matrix33 = _settings.WebcamOverlay.Opacity / 100.0f
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