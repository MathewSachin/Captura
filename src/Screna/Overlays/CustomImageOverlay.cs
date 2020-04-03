using System;
using System.Drawing;

namespace Captura.Video
{
    public class CustomImageOverlay : ImageOverlay
    {
        IBitmapImage _bmp;
        readonly CustomImageOverlaySettings _settings;

        public CustomImageOverlay(CustomImageOverlaySettings ImageOverlaySettings)
            : base(false)
        {
            _settings = ImageOverlaySettings;
        }

        public override void Draw(IEditableFrame Editor, Func<Point, Point> PointTransform = null)
        {
            var img = GetImage(Editor);

            var size = new Size(img.Width, img.Height);

            if (_settings.Resize)
                size = new Size(_settings.ResizeWidth, _settings.ResizeHeight);

            var pos = GetPosition(new SizeF(Editor.Width, Editor.Height), size);

            Draw(Editor, img, pos, size, _settings.Opacity);
        }

        public override void Dispose()
        {
            _bmp?.Dispose();
        }

        PointF GetPosition(SizeF Bounds, Size ImageSize)
        {
            var point = new PointF(_settings.X, _settings.Y);

            switch (_settings.HorizontalAlignment)
            {
                case Alignment.Center:
                    point.X = Bounds.Width / 2 - ImageSize.Width / 2f + point.X;
                    break;

                case Alignment.End:
                    point.X = Bounds.Width - ImageSize.Width - point.X;
                    break;
            }

            switch (_settings.VerticalAlignment)
            {
                case Alignment.Center:
                    point.Y = Bounds.Height / 2 - ImageSize.Height / 2f + point.Y;
                    break;

                case Alignment.End:
                    point.Y = Bounds.Height - ImageSize.Height - point.Y;
                    break;
            }

            return point;
        }

        IBitmapImage GetImage(IEditableFrame Editor)
        {
            return _bmp ??= Editor.LoadBitmap(_settings.Source);
        }
    }
}