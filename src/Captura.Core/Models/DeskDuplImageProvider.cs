using System.Drawing;
using Screna;
using DesktopDuplication;

namespace Captura.Models
{
    public class DeskDuplImageProvider : IImageProvider
    {
        Rectangle _rect;
        DesktopDuplicator _dupl;

        public DeskDuplImageProvider(Rectangle Rect)
        {
            _rect = Rect;

            Width = Rect.Width;
            Height = Rect.Height;            
        }

        public DeskDuplImageProvider()
            : this(WindowProvider.DesktopRectangle) { }

        public int Height { get; }

        public int Width { get; }

        public Bitmap Capture()
        {
            try
            {
                return _dupl.GetLatestFrame(_rect);
            }
            catch
            {
                _dupl = new DesktopDuplicator(0);

                return _dupl.GetLatestFrame(_rect);
            }
        }

        public void Dispose() { }
    }
}
