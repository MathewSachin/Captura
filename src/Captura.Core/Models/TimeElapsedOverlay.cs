using Screna;
using System;
using System.Drawing;

namespace Captura.Models
{
    public class TimeElapsedOverlay : IOverlay
    {
        readonly Func<TimeSpan> _elapsed;

        public TimeElapsedOverlay(Func<TimeSpan> Elapsed)
        {
            _elapsed = Elapsed;
        }

        static float GetLeft(float FullWidth, float TextWidth)
        {
            var x = Settings.Instance.TimeElapsedOverlay.X;

            switch (Settings.Instance.TimeElapsedOverlay.HorizontalAlignment)
            {
                case Alignment.Start:
                    return x;

                case Alignment.End:
                    return FullWidth - x - TextWidth - 2 * Settings.Instance.TimeElapsedOverlay.HorizontalPadding;

                case Alignment.Center:
                    return FullWidth / 2 + x - TextWidth / 2 - Settings.Instance.TimeElapsedOverlay.HorizontalPadding;

                default:
                    return 0;
            }
        }

        static float GetTop(float FullHeight, float TextHeight)
        {
            var y = Settings.Instance.TimeElapsedOverlay.Y;

            switch (Settings.Instance.TimeElapsedOverlay.VerticalAlignment)
            {
                case Alignment.Start:
                    return y;

                case Alignment.End:
                    return FullHeight - y - TextHeight - 2 * Settings.Instance.TimeElapsedOverlay.VerticalPadding;

                case Alignment.Center:
                    return FullHeight / 2 + y - TextHeight / 2 - Settings.Instance.TimeElapsedOverlay.VerticalPadding;

                default:
                    return 0;
            }
        }
        
        /// <summary>
        /// Frees all resources used by this object.
        /// </summary>
        public void Dispose() { }
        
        public void Draw(Graphics g, Func<Point, Point> PointTransform = null)
        {
            var text = _elapsed().ToString();
            var fontSize = Settings.Instance.TimeElapsedOverlay.FontSize;

            var font = new Font(FontFamily.GenericMonospace, fontSize);

            var size = g.MeasureString(text, font);

            int paddingX = Settings.Instance.TimeElapsedOverlay.HorizontalPadding, paddingY = Settings.Instance.TimeElapsedOverlay.VerticalPadding;

            var rect = new RectangleF(GetLeft(g.VisibleClipBounds.Width, size.Width),
                GetTop(g.VisibleClipBounds.Height, size.Height),
                size.Width + 2 * paddingX,
                size.Height + 2 * paddingY);

            g.FillRoundedRectangle(new SolidBrush(Settings.Instance.TimeElapsedOverlay.BackgroundColor),
                rect,
                Settings.Instance.TimeElapsedOverlay.CornerRadius);

            g.DrawString(text,
                font,
                new SolidBrush(Settings.Instance.TimeElapsedOverlay.FontColor),
                new RectangleF(rect.Left + paddingX, rect.Top + paddingY, size.Width, size.Height));

            var border = Settings.Instance.TimeElapsedOverlay.BorderThickness;

            if (border > 0)
            {
                rect = new RectangleF(rect.Left - border / 2, rect.Top - border / 2, rect.Width + border, rect.Height + border);

                g.DrawRoundedRectangle(new Pen(Settings.Instance.TimeElapsedOverlay.BorderColor, border),
                    rect,
                    Settings.Instance.TimeElapsedOverlay.CornerRadius);
            }
        }
    }
}
