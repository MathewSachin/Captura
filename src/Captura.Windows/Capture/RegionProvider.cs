using System;
using System.Drawing;
using Captura;

namespace Screna
{
    class RegionProvider : IImageProvider
    {
        Rectangle _region;
        readonly Func<Point> _locationFunc;
        readonly bool _includeCursor;

        readonly IntPtr _hdcSrc, _hdcDest, _hBitmap;

        public RegionProvider(Rectangle Region, bool IncludeCursor, Func<Point> LocationFunc = null)
        {
            _region = Region;
            _includeCursor = IncludeCursor;
            _locationFunc = LocationFunc ?? (() => Region.Location);

            // Width and Height must be even.
            // Use these for Bitmap size, but capture as per region size
            Width = _region.Width;
            if (Width % 2 == 1)
                ++Width;
            
            Height = _region.Height;
            if (Height % 2 == 1)
                ++Height;

            PointTransform = P => new Point(P.X - _region.X, P.Y - _region.Y);

            _hdcSrc = User32.GetDC(IntPtr.Zero);

            _hdcDest = Gdi32.CreateCompatibleDC(_hdcSrc);
            _hBitmap = Gdi32.CreateCompatibleBitmap(_hdcSrc, Width, Height);

            Gdi32.SelectObject(_hdcDest, _hBitmap);
        }

        public void Dispose()
        {
            Gdi32.DeleteDC(_hdcDest);
            User32.ReleaseDC(IntPtr.Zero, _hdcSrc);
            Gdi32.DeleteObject(_hBitmap);
        }

        public IEditableFrame Capture()
        {
            // Update Location
            _region.Location = _locationFunc();

            Gdi32.BitBlt(_hdcDest, 0, 0, _region.Width, _region.Height,
                _hdcSrc, _region.X, _region.Y,
                (int) CopyPixelOperation.SourceCopy);

            var img = new GraphicsEditor(Image.FromHbitmap(_hBitmap));

            if (_includeCursor)
                MouseCursor.Draw(img, PointTransform);

            return img;
        }

        public Func<Point, Point> PointTransform { get; }

        public int Height { get; }
        public int Width { get; }
    }
}
