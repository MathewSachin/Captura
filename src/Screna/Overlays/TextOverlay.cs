using System;
using System.Drawing;

namespace Captura.Video
{
    public abstract class TextOverlay : IOverlay
    {
        readonly TextOverlaySettings _overlaySettings;

        protected TextOverlay(TextOverlaySettings OverlaySettings)
        {
            _overlaySettings = OverlaySettings;
        }

        public virtual void Dispose() { }

        static float GetLeft(TextOverlaySettings OverlaySettings, float FullWidth, float TextWidth)
        {
            var x = OverlaySettings.X;

            switch (OverlaySettings.HorizontalAlignment)
            {
                case Alignment.Start:
                    return x;

                case Alignment.End:
                    return FullWidth - x - TextWidth - 2 * OverlaySettings.HorizontalPadding;

                case Alignment.Center:
                    return FullWidth / 2 + x - TextWidth / 2 - OverlaySettings.HorizontalPadding;

                default:
                    return 0;
            }
        }

        static float GetTop(TextOverlaySettings OverlaySettings, float FullHeight, float TextHeight)
        {
            var y = OverlaySettings.Y;

            switch (OverlaySettings.VerticalAlignment)
            {
                case Alignment.Start:
                    return y;

                case Alignment.End:
                    return FullHeight - y - TextHeight - 2 * OverlaySettings.VerticalPadding;

                case Alignment.Center:
                    return FullHeight / 2 + y - TextHeight / 2 - OverlaySettings.VerticalPadding;

                default:
                    return 0;
            }
        }

        protected abstract string GetText();
        
        public virtual void Draw(IEditableFrame Editor, Func<Point, Point> PointTransform = null)
        {
            if (!_overlaySettings.Display)
                return;

            var text = GetText();

            if (string.IsNullOrWhiteSpace(text))
                return;

            var font = Editor.GetFont(_overlaySettings.FontFamily, _overlaySettings.FontSize);

            var size = Editor.MeasureString(text, font);

            int paddingX = _overlaySettings.HorizontalPadding, paddingY = _overlaySettings.VerticalPadding;

            var rect = new RectangleF(GetLeft(_overlaySettings, Editor.Width, size.Width),
                GetTop(_overlaySettings, Editor.Height, size.Height),
                size.Width + 2 * paddingX,
                size.Height + 2 * paddingY);

            Editor.FillRectangle(_overlaySettings.BackgroundColor,
                rect,
                _overlaySettings.CornerRadius);

            Editor.DrawString(text,
                font,
                _overlaySettings.FontColor,
                new RectangleF(rect.Left + paddingX, rect.Top + paddingY, size.Width, size.Height));

            var border = _overlaySettings.BorderThickness;

            if (border > 0)
            {
                rect = new RectangleF(rect.Left - border / 2f, rect.Top - border / 2f, rect.Width + border, rect.Height + border);

                Editor.DrawRectangle(_overlaySettings.BorderColor, border,
                    rect,
                    _overlaySettings.CornerRadius);
            }
        }
    }
}