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
            var x = Settings.Instance.TimeElapsed.X;

            switch (Settings.Instance.TimeElapsed.HorizontalAlignment)
            {
                case Alignment.Start:
                    return x;

                case Alignment.End:
                    return FullWidth - x - TextWidth - 2 * Settings.Instance.TimeElapsed.HorizontalPadding;

                case Alignment.Center:
                    return FullWidth / 2 + x - TextWidth / 2 - Settings.Instance.TimeElapsed.HorizontalPadding;

                default:
                    return 0;
            }
        }

        static float GetTop(float FullHeight, float TextHeight)
        {
            var y = Settings.Instance.TimeElapsed.Y;

            switch (Settings.Instance.TimeElapsed.VerticalAlignment)
            {
                case Alignment.Start:
                    return y;

                case Alignment.End:
                    return FullHeight - y - TextHeight - 2 * Settings.Instance.TimeElapsed.VerticalPadding;

                case Alignment.Center:
                    return FullHeight / 2 + y - TextHeight / 2 - Settings.Instance.TimeElapsed.VerticalPadding;

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
            var fontSize = Settings.Instance.TimeElapsed.FontSize;

            var font = new Font(FontFamily.GenericMonospace, fontSize);

            var size = g.MeasureString(text, font);

            int paddingX = Settings.Instance.TimeElapsed.HorizontalPadding, paddingY = Settings.Instance.TimeElapsed.VerticalPadding;

            var rect = new RectangleF(GetLeft(g.VisibleClipBounds.Width, size.Width),
                GetTop(g.VisibleClipBounds.Height, size.Height),
                size.Width + 2 * paddingX,
                size.Height + 2 * paddingY);

            g.FillRoundedRectangle(new SolidBrush(Settings.Instance.TimeElapsed.BackgroundColor),
                rect,
                Settings.Instance.TimeElapsed.CornerRadius);

            g.DrawString(text,
                font,
                new SolidBrush(Settings.Instance.TimeElapsed.FontColor),
                new RectangleF(rect.Left + paddingX, rect.Top + paddingY, size.Width, size.Height));

            var border = Settings.Instance.TimeElapsed.BorderThickness;

            if (border > 0)
            {
                rect = new RectangleF(rect.Left - border / 2, rect.Top - border / 2, rect.Width + border, rect.Height + border);

                g.DrawRoundedRectangle(new Pen(Settings.Instance.TimeElapsed.BorderColor, border),
                    rect,
                    Settings.Instance.TimeElapsed.CornerRadius);
            }
        }
    }
}
