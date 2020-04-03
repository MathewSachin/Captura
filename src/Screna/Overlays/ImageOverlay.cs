using System;
using System.Drawing;

namespace Captura.Video
{
    public abstract class ImageOverlay : IOverlay
    {
        readonly bool _disposeImages;

        protected ImageOverlay(bool DisposeImages)
        {
            _disposeImages = DisposeImages;
        }

        public abstract void Draw(IEditableFrame Editor, Func<Point, Point> PointTransform = null);

        protected void Draw(IEditableFrame Editor, IBitmapImage Image, PointF Position, SizeF? Size, int Opacity)
        {
            if (Image == null)
                return;

            var targetSize = Size ?? new Size(Image.Width, Image.Height);

            try
            {
                //var point = GetPosition(new Size((int)Editor.Width, (int)Editor.Height), targetSize);
                var destRect = new RectangleF(Position, targetSize);

                Editor.DrawImage(Image, destRect, Opacity);
            }
            catch { }
            finally
            {
                if (_disposeImages)
                    Image.Dispose();
            }
        }

        public virtual void Dispose() { }
    }
}