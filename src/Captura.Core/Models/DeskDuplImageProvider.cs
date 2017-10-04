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
        }
        
        public int Height { get; }

        public int Width { get; }

        public Bitmap Capture()
        {
            if (_dupl == null)
                _dupl = new DesktopDuplicator(_rectangle, _includeCursor, _monitor);

            try
            {
                return _dupl.Capture();
            }
            catch
            {
                _dupl?.Dispose();

                _dupl = new DesktopDuplicator(_rectangle, _includeCursor, _monitor);

                try
                {
                    return _dupl.Capture();
                }
                catch
                {
                    return new Bitmap(Width, Height);
                }
            }
        }

        public void Dispose()
        {
            _dupl?.Dispose();
        }
    }
}
