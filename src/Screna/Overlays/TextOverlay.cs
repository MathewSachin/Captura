using System;
using System.Drawing;

namespace Captura.Models
{
    public abstract class TextOverlay : IOverlay
    {
        readonly TextOverlaySettings _overlaySettings;

        protected TextOverlay(TextOverlaySettings OverlaySettings)
        {
            _overlaySettings = OverlaySettings;
        }

        public virtual void Dispose() { }

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

            var x = _overlaySettings.GetX(Editor.Width, size.Width);
            var y = _overlaySettings.GetY(Editor.Height, size.Height);

            var rect = new RectangleF((float)x,
                (float)y,
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