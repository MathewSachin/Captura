using System;
using System.Drawing;
using Captura;
using Captura.Native;

namespace Screna
{
    public class RegionProvider : IImageProvider
    {
        Rectangle _region;
        readonly bool _includeCursor;
        readonly Func<Point, Point> _transform;

        readonly IntPtr _hdcSrc, _hdcDest, _hBitmap;

        public RegionProvider(Rectangle Region, bool IncludeCursor)
        {
            _region = Region;
            _includeCursor = IncludeCursor;

            // Width and Height must be even.
            // Use these for Bitmap size, but capture as per region size
            Width = _region.Width;
            if (Width % 2 == 1)
                ++Width;
            
            Height = _region.Height;
            if (Height % 2 == 1)
                ++Height;

            _transform = P => new Point(P.X - _region.X, P.Y - _region.Y);

            _hdcSrc = User32.GetDC(IntPtr.Zero);

            _hdcDest = Gdi32.CreateCompatibleDC(_hdcSrc);
            _hBitmap = Gdi32.CreateCompatibleBitmap(_hdcSrc, Width, Height);

            Gdi32.SelectObject(_hdcDest, _hBitmap);
        }

        public void UpdateLocation(Point P)
        {
            if (_region.Location == P)
                return;

            _region.Location = P;
        }

        public void Dispose()
        {
            Gdi32.DeleteDC(_hdcDest);
            User32.ReleaseDC(IntPtr.Zero, _hdcSrc);
            Gdi32.DeleteObject(_hBitmap);
        }

        public IBitmapFrame Capture()
        {
            Gdi32.BitBlt(_hdcDest, 0, 0, _region.Width, _region.Height,
                _hdcSrc, _region.X, _region.Y,
                (int) CopyPixelOperation.SourceCopy);

            var img = new OneTimeFrame(Image.FromHbitmap(_hBitmap));

            if (_includeCursor)
                using (var editor = img.GetEditor())
                    MouseCursor.Draw(editor.Graphics, _transform);

            return img;
        }

        public int Height { get; }
        public int Width { get; }
    }
}
