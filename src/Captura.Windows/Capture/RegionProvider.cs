using System;
using System.Drawing;
using Captura;
using Captura.Models;
using DesktopDuplication;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace Screna
{
    class RegionProvider : IImageProvider
    {
        Rectangle _region;
        readonly Func<Point> _locationFunc;
        readonly bool _includeCursor;

        readonly IntPtr _hdcSrc, _hBitmap;
        readonly Direct2DEditorSession _editorSession;
        readonly Texture2D _gdiCompatibleTexture;
        readonly Surface1 _dxgiSurface;

        IntPtr _hdcDest;

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

            if (WindowsModule.Windows8OrAbove)
            {
                _editorSession = new Direct2DEditorSession(Width, Height, PreviewWindow);
                _gdiCompatibleTexture = _editorSession.CreateGdiTexture(Width, Height);

                _dxgiSurface = _gdiCompatibleTexture.QueryInterface<Surface1>();

                EditorType = typeof(Direct2DEditor);
            }
            else
            {
                _hdcDest = Gdi32.CreateCompatibleDC(_hdcSrc);
                _hBitmap = Gdi32.CreateCompatibleBitmap(_hdcSrc, Width, Height);

                Gdi32.SelectObject(_hdcDest, _hBitmap);

                EditorType = typeof(GraphicsEditor);
            }
        }

        public void Dispose()
        {
            if (_dxgiSurface != null)
            {
                _dxgiSurface.Dispose();
                _gdiCompatibleTexture.Dispose();
                _editorSession.Dispose();
            }
            else
            {
                Gdi32.DeleteDC(_hdcDest);
                Gdi32.DeleteObject(_hBitmap);
            }

            User32.ReleaseDC(IntPtr.Zero, _hdcSrc);            
        }

        public IEditableFrame Capture()
        {
            // Update Location
            _region.Location = _locationFunc();

            if (_dxgiSurface != null)
            {
                _hdcDest = _dxgiSurface.GetDC(true);
            }

            Gdi32.BitBlt(_hdcDest, 0, 0, _region.Width, _region.Height,
                _hdcSrc, _region.X, _region.Y,
                (int) CopyPixelOperation.SourceCopy);

            IEditableFrame img;

            if (_dxgiSurface != null)
            {
                _dxgiSurface.ReleaseDC();

                _editorSession.Device.ImmediateContext.CopyResource(_gdiCompatibleTexture, _editorSession.DesktopTexture);
                _editorSession.Device.ImmediateContext.Flush();

                img = new Direct2DEditor(_editorSession);
            }
            else img = new GraphicsEditor(Image.FromHbitmap(_hBitmap));

            if (_includeCursor)
                MouseCursor.Draw(img, PointTransform);

            return img;
        }

        public Func<Point, Point> PointTransform { get; }

        public int Height { get; }
        public int Width { get; }

        public Type EditorType { get; }
    }
}
