using System.Drawing;
using Screna;
using DesktopDuplication;

namespace Captura.Models
{
    public class DeskDuplImageProvider : IImageProvider
    {
        DesktopDuplicator _dupl;
        readonly int _monitor;
        readonly bool _includeCursor;
        readonly Rectangle _rectangle;

        public DeskDuplImageProvider(int Monitor, Rectangle Rectangle, bool IncludeCursor)
        {
            _monitor = Monitor;
            _includeCursor = IncludeCursor;
            _rectangle = Rectangle;

            Width = Rectangle.Width;
            Height = Rectangle.Height;

            _imagePool = new ImagePool(Width, Height);
        }
        
        public int Height { get; }

        public int Width { get; }

        readonly ImagePool _imagePool;

        public ImageWrapper Capture()
        {
            if (_dupl == null)
                _dupl = new DesktopDuplicator(_rectangle, _includeCursor, _monitor);

            var img = _imagePool.Get();

            try
            {
                return _dupl.Capture(img);
            }
            catch
            {
                _dupl?.Dispose();

                _dupl = new DesktopDuplicator(_rectangle, _includeCursor, _monitor);

                try
                {
                    return _dupl.Capture(img);
                }
                catch
                {
                    img.Written = true;
                    return ImageWrapper.Repeat;
                }
            }
        }

        public void Dispose()
        {
            _dupl?.Dispose();
            _imagePool.Dispose();
        }
    }
}
