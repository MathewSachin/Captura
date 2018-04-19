using System;
using System.Drawing;
using System.Drawing.Imaging;
using Screna;

namespace Captura.Models
{
    public abstract class ImageOverlay<T> : IOverlay where T : ImageOverlaySettings
    {
        protected readonly T Settings;
        readonly bool _disposeImages;

        protected ImageOverlay(T Settings, bool DisposeImages)
        {
            _disposeImages = DisposeImages;

            this.Settings = Settings;
        }

        public void Draw(Graphics g, Func<Point, Point> PointTransform = null)
        {
            var img = GetImage();

            if (img == null)
                return;

            if (Settings.Resize)
                img = img.Resize(new Size(Settings.ResizeWidth, Settings.ResizeHeight), false, _disposeImages);

            try
            {
                var point = GetPosition(new Size((int) g.VisibleClipBounds.Width, (int) g.VisibleClipBounds.Height), img.Size);

                if (Settings.Opacity < 100)
                {
                    var colormatrix = new ColorMatrix
                    {
                        Matrix33 = Settings.Opacity / 100.0f
                    };

                    var imgAttribute = new ImageAttributes();
                    imgAttribute.SetColorMatrix(colormatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                    g.DrawImage(img, new Rectangle(point, img.Size), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imgAttribute);
                }
                else g.DrawImage(img, point);
            }
            catch { }
            finally
            {
                if (_disposeImages)
                    img.Dispose();
            }
        }

        public abstract Bitmap GetImage();

        Point GetPosition(Size Bounds, Size ImageSize)
        {
            var point = new Point(Settings.X, Settings.Y);

            switch (Settings.HorizontalAlignment)
            {
                case Alignment.Center:
                    point.X = Bounds.Width / 2 - ImageSize.Width / 2 + point.X;
                    break;

                case Alignment.End:
                    point.X = Bounds.Width - ImageSize.Width - point.X;
                    break;
            }

            switch (Settings.VerticalAlignment)
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