using DesktopDuplication;
using System;
using System.Drawing;

namespace Captura.Models
{
    class DeskDuplFullScreenImageProvider : IImageProvider
    {
        readonly FullScreenDesktopDuplicator _dupl;

        public DeskDuplFullScreenImageProvider(bool IncludeCursor,
            IPreviewWindow PreviewWindow,
            IPlatformServices PlatformServices)
        {
            _dupl = new FullScreenDesktopDuplicator(IncludeCursor, PreviewWindow, PlatformServices);
        }

        public int Height => _dupl.Height;

        public int Width => _dupl.Width;

        public Func<Point, Point> PointTransform => _dupl.PointTransform;
        
        public IEditableFrame Capture()
        {
            return _dupl.Capture();
        }

        public void Dispose()
        {
            _dupl?.Dispose();
        }

        public IBitmapFrame DummyFrame => Texture2DFrame.DummyFrame;
    }
}
