using System;
using System.Drawing;
using Captura.Native;
using Captura.Windows;
using Captura.Windows.Gdi;

namespace Captura.Video
{
    class RegionProvider : IImageProvider
    {
        Rectangle _region;
        readonly Func<Point> _locationFunc;
        readonly bool _includeCursor;

        readonly IntPtr _hdcSrc;
        readonly ITargetDeviceContext _dcTarget;

        public RegionProvider(Rectangle Region,
            IPreviewWindow PreviewWindow,
            bool IncludeCursor,
            Func<Point> LocationFunc = null)
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

            if (WindowsModule.ShouldUseGdi)
            {
                _dcTarget = new GdiTargetDeviceContext(_hdcSrc, Width, Height);
            }
            else _dcTarget = new DxgiTargetDeviceContext(PreviewWindow, Width, Height);
        }

        public void Dispose()
        {
            _dcTarget.Dispose();

            User32.ReleaseDC(IntPtr.Zero, _hdcSrc);            
        }

        public IEditableFrame Capture()
        {
            // Update Location
            _region.Location = _locationFunc();

            var hdcDest = _dcTarget.GetDC();

            Gdi32.BitBlt(hdcDest, 0, 0, _region.Width, _region.Height,
                _hdcSrc, _region.X, _region.Y,
                (int) CopyPixelOperation.SourceCopy);

            if (_includeCursor)
                MouseCursor.Draw(hdcDest, PointTransform);

            var img = _dcTarget.GetEditableFrame();

            return img;
        }

        public Func<Point, Point> PointTransform { get; }

        public int Height { get; }
        public int Width { get; }

        public IBitmapFrame DummyFrame => _dcTarget.DummyFrame;
    }
}
