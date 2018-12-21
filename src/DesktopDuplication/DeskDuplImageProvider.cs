using System.Drawing;
using DesktopDuplication;
using SharpDX.DXGI;

namespace Captura.Models
{
    public class DeskDuplImageProvider : IImageProvider
    {
        readonly DesktopDuplicator _dupl;

        public int Timeout { get; set; }

        internal DeskDuplImageProvider(Adapter Adapter, Output1 Output, Rectangle Rectangle, bool IncludeCursor)
        {
            Width = Rectangle.Width;
            Height = Rectangle.Height;

            _dupl = new DesktopDuplicator(Rectangle, IncludeCursor, Adapter, Output)
            {
                Timeout = Timeout
            };
        }

        public int Height { get; }

        public int Width { get; }
        
        public IBitmapFrame Capture()
        {
            return _dupl.Capture();
        }

        public void Dispose()
        {
            _dupl?.Dispose();
        }
    }
}
