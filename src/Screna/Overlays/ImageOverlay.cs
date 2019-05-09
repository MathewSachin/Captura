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
                var targetSize = new Size((int)Settings.GetWidth(Editor.Width), (int)Settings.GetHeight(Editor.Height));

                var x = Settings.GetX(Editor.Width, targetSize.Width);
                var y = Settings.GetY(Editor.Height, targetSize.Height);

                var destRect = new Rectangle(new Point((int)x, (int)y), targetSize);

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

        public virtual void Dispose() { }
    }
}