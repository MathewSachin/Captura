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
            _region = Region.Even();
            _includeCursor = IncludeCursor;

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
            Gdi32.BitBlt(_hdcDest, 0, 0, Width, Height,
                _hdcSrc, _region.X, _region.Y,
                (int) CopyPixelOperation.SourceCopy);

            var img = new OneTimeFrame(Image.FromHbitmap(_hBitmap));

            if (_includeCursor)
                using (var editor = img.GetEditor())
                    MouseCursor.Draw(editor.Graphics, _transform);

            return img;
        }

        public int Height => _region.Height;
        public int Width => _region.Width;
    }
}
