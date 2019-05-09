using System;
using System.Drawing;

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

        public void Draw(IEditableFrame Editor, Func<Point, Point> PointTransform = null)
        {
            var img = GetImage(Editor);

            if (img == null)
                return;

            try
            {
                var targetSize =new Size((int)Settings.GetWidth(Editor.Width), (int)Settings.GetHeight(Editor.Height));

                var point = GetPosition(new Size((int)Editor.Width, (int)Editor.Height), targetSize);
                var destRect = new Rectangle(point, targetSize);

                Editor.DrawImage(img, destRect, Settings.Opacity);
            }
            catch { }
            finally
            {
                if (_disposeImages)
                    img.Dispose();
            }
        }

        protected abstract IBitmapImage GetImage(IEditableFrame Editor);

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