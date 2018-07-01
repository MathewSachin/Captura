using System;
using System.Drawing;
using System.Drawing.Imaging;
using Screna;

namespace Captura.Models
{
    public abstract class ImageOverlay<T> : IOverlay where T : ImageOverlaySettings
    {
        readonly T _settings;
        readonly bool _disposeImages;

        protected ImageOverlay(T Settings, bool DisposeImages)
        {
            _disposeImages = DisposeImages;

            _settings = Settings;
        }

        public void Draw(Graphics G, Func<Point, Point> PointTransform = null)
        {
            var img = GetImage();

            if (img == null)
                return;

            if (_settings.Resize)
                img = img.Resize(new Size(_settings.ResizeWidth, _settings.ResizeHeight), false, _disposeImages);

            try
            {
                var point = GetPosition(new Size((int) G.VisibleClipBounds.Width, (int) G.VisibleClipBounds.Height), img.Size);

                if (_settings.Opacity < 100)
                {
                    var colormatrix = new ColorMatrix
                    {
                        Matrix33 = _settings.Opacity / 100.0f
                    };

                    var imgAttribute = new ImageAttributes();
                    imgAttribute.SetColorMatrix(colormatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                    G.DrawImage(img, new Rectangle(point, img.Size), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imgAttribute);
                }
                else G.DrawImage(img, point);
            }
            catch { }
            finally
            {
                if (_disposeImages)
                    img.Dispose();
            }
        }

        protected abstract Bitmap GetImage();

        Point GetPosition(Size Bounds, Size ImageSize)
        {
            var point = new Point(_settings.X, _settings.Y);

            switch (_settings.HorizontalAlignment)
            {
                case Alignment.Center:
                    point.X = Bounds.Width / 2 - ImageSize.Width / 2 + point.X;
                    break;

                case Alignment.End:
                    point.X = Bounds.Width - ImageSize.Width - point.X;
                    break;
            }

            switch (_settings.VerticalAlignment)
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

        public virtual void Dispose() { }
    }
}