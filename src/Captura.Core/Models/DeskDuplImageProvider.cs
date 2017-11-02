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

            Reinit();
        }

        void Reinit()
        {
            _dupl?.Dispose();

            _dupl = new DesktopDuplicator(_rectangle, _includeCursor, _monitor);
        }
        
        public int Height { get; }

        public int Width { get; }

        readonly ImagePool _imagePool;

        public Frame Capture()
        {
            try
            {
                return _dupl.Capture(_imagePool.Get);
            }
            catch
            {
                Reinit();

                try
                {
                    return _dupl.Capture(_imagePool.Get);
                }
                catch
                {
                    return Frame.Repeat;
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
