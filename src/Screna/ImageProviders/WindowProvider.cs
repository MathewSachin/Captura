﻿using System;
using System.Drawing;
using System.Windows.Forms;
using Captura;
using Captura.Models;
using Captura.Native;

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
        public static Rectangle DesktopRectangle => SystemInformation.VirtualScreen;

        readonly IWindow _window;
        readonly Func<Point, Point> _transform;
        readonly bool _includeCursor;

        readonly IntPtr _hdcSrc, _hdcDest, _hBitmap;

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

            _hdcSrc = User32.GetDC(IntPtr.Zero);

            _hdcDest = Gdi32.CreateCompatibleDC(_hdcSrc);
            _hBitmap = Gdi32.CreateCompatibleBitmap(_hdcSrc, Width, Height);

            Gdi32.SelectObject(_hdcDest, _hBitmap);
        }

        void OnCapture()
        {
            if (!_window.IsAlive)
            {
                throw new WindowClosedException();
            }

            var rect = _window.Rectangle.Even();
            var ratio = Math.Min((float) Width / rect.Width, (float) Height / rect.Height);

            var resizeWidth = (int) (rect.Width * ratio);
            var resizeHeight = (int) (rect.Height * ratio);

            void ClearRect(RECT Rect)
            {
                User32.FillRect(_hdcDest, ref Rect, IntPtr.Zero);
            }

            if (Width != resizeWidth)
            {
                ClearRect(new RECT
                {
                    Left = resizeWidth,
                    Right = Width,
                    Bottom = Height
                });
            }
            else if (Height != resizeHeight)
            {
                ClearRect(new RECT
                {
                    Top = resizeHeight,
                    Right = Width,
                    Bottom = Height
                });
            }

            Gdi32.StretchBlt(_hdcDest, 0, 0, resizeWidth, resizeHeight,
                _hdcSrc, rect.X, rect.Y, rect.Width, rect.Height,
                (int) CopyPixelOperation.SourceCopy);
        }

        public IBitmapFrame Capture()
        {
            try
            {
                OnCapture();

                var img = new OneTimeFrame(Image.FromHbitmap(_hBitmap));

                if (_includeCursor)
                    using (var editor = img.GetEditor())
                        MouseCursor.Draw(editor.Graphics, _transform);

                return img;
            }
            catch (Exception e) when (!(e is WindowClosedException))
            {
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
            Gdi32.DeleteDC(_hdcDest);
            User32.ReleaseDC(IntPtr.Zero, _hdcSrc);
            Gdi32.DeleteObject(_hBitmap);
        }
    }
}
