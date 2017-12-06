using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Screna
{
    /// <summary>
    /// Captures the specified window.
    /// </summary>
    public class WindowProvider : ImageProviderBase
    {
        /// <summary>
        /// A <see cref="Rectangle"/> representing the entire Desktop.
        /// </summary>
        public static Rectangle DesktopRectangle { get; private set; }

        static WindowProvider()
        {
            RefreshDesktopSize();
        }

        public static void RefreshDesktopSize()
        {
            var height = 0;
            var width = 0;

            foreach (var screen in Screen.AllScreens)
            {
                var w = screen.Bounds.X + screen.Bounds.Width;
                var h = screen.Bounds.Y + screen.Bounds.Height;

                if (height < h)
                    height = h;

                if (width < w)
                    width = w;
            }

            DesktopRectangle = new Rectangle(0, 0, width, height);
        }

        readonly Window _window;
        readonly Color _backgroundColor;

        static Func<Point, Point> GetTransformer(Window Window)
        {
            var initialSize = Window.Rectangle.Even().Size;

            return P =>
            {
                var rect = Window.Rectangle;
                
                var ratio = Math.Min((float)initialSize.Width / rect.Width, (float)initialSize.Height / rect.Height);

                return new Point((int)((P.X - rect.X) * ratio), (int)((P.Y - rect.Y) * ratio));
            };
        }
        
        /// <summary>
        /// Creates a new instance of <see cref="WindowProvider"/>.
        /// </summary>
        /// <param name="Window">The Window to Capture.</param>
        /// <param name="BackgroundColor"><see cref="Color"/> to fill blank background.</param>
        public WindowProvider(Window Window, bool IncludeCursor, Color BackgroundColor, out Func<Point, Point> Transform)
            : base(Window.Rectangle.Even().Size, GetTransformer(Window), IncludeCursor)
        {
            _window = Window;
            _backgroundColor = BackgroundColor;

            Transform = _transform;
        }

        /// <summary>
        /// Capture Image.
        /// </summary>
        protected override void OnCapture(Graphics g)
        {
            var rect = _window.Rectangle;
            
            if (rect.Width == Width && rect.Height == Height)
            {
                g.CopyFromScreen(rect.Location,
                    Point.Empty,
                    rect.Size,
                    CopyPixelOperation.SourceCopy);
            }
            else // Scale to fit
            {
                var capture = new Bitmap(rect.Width, rect.Height);

                using (var gcapture = Graphics.FromImage(capture))
                {
                    gcapture.CopyFromScreen(rect.Location,
                        Point.Empty,
                        rect.Size,
                        CopyPixelOperation.SourceCopy);
                }

                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                
                if (_backgroundColor != Color.Transparent)
                    g.FillRectangle(new SolidBrush(_backgroundColor), 0, 0, Width, Height);

                var ratio = Math.Min((float)Width / rect.Width, (float)Height / rect.Height);

                var resizeWidth = rect.Width * ratio;
                var resizeHeight = rect.Height * ratio;

                using (capture)
                    g.DrawImage(capture, 0, 0, resizeWidth, resizeHeight);
            }
        }
    }
}
