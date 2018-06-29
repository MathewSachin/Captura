using System;
using System.Drawing;
using System.Windows.Forms;
using Captura;
using Captura.Models;

namespace Screna
{
    /// <summary>
    /// Captures the specified window.
    /// </summary>
    public class WindowProvider : IImageProvider
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

            if (_fullWidthFrame != null)
                _fullWidthFrame.Destroy();
            else
            {
                _fullWidthFrame = new ReusableFrame(new Bitmap(width, height));
            }
        }

        readonly IWindow _window;
        readonly Func<Point, Point> _transform;
        readonly bool _includeCursor;
        readonly ImagePool _imagePool;

        // used when resizing window frames.
        static ReusableFrame _fullWidthFrame;

        static Func<Point, Point> GetTransformer(IWindow Window)
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
        public WindowProvider(IWindow Window, bool IncludeCursor, out Func<Point, Point> Transform)
        {
            _window = Window ?? throw new ArgumentNullException(nameof(Window));
            _includeCursor = IncludeCursor;

            var size = Window.Rectangle.Even().Size;
            Width = size.Width;
            Height = size.Height;

            Transform = _transform = GetTransformer(Window);

            _imagePool = new ImagePool(Width, Height);
        }

        void OnCapture(Graphics G)
        {
            if (!_window.IsAlive)
            {
                throw new OperationCanceledException();
            }

            var rect = _window.Rectangle.Even();
            
            if (rect.Width == Width && rect.Height == Height)
            {
                G.CopyFromScreen(rect.Location,
                    Point.Empty,
                    rect.Size,
                    CopyPixelOperation.SourceCopy);
            }
            else // Scale to fit
            {
                using (var editor = _fullWidthFrame.GetEditor())
                {
                    editor.Graphics.CopyFromScreen(rect.Location,
                        Point.Empty,
                        rect.Size,
                        CopyPixelOperation.SourceCopy);
                }
                
                var ratio = Math.Min((float)Width / rect.Width, (float)Height / rect.Height);

                var resizeWidth = rect.Width * ratio;
                var resizeHeight = rect.Height * ratio;

                G.Clear(Color.Transparent);
                
                G.DrawImage(_fullWidthFrame.Bitmap,
                    new RectangleF(0, 0, resizeWidth, resizeHeight),
                    new RectangleF(0, 0, rect.Width, rect.Height), 
                    GraphicsUnit.Pixel);
            }
        }

        public IBitmapFrame Capture()
        {
            var bmp = _imagePool.Get();

            try
            {
                using (var editor = bmp.GetEditor())
                {
                    OnCapture(editor.Graphics);

                    if (_includeCursor)
                        MouseCursor.Draw(editor.Graphics, _transform);
                }

                return bmp;
            }
            catch (Exception e) when (!(e is OperationCanceledException))
            {
                bmp.Dispose();

                return RepeatFrame.Instance;
            }
        }

        /// <summary>
        /// Height of Captured image.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Width of Captured image.
        /// </summary>
        public int Width { get; }

        public void Dispose()
        {
            _imagePool.Dispose();
        }
    }
}
