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

        public int Timeout { get; set; }

        public DeskDuplImageProvider(int Monitor, Rectangle Rectangle, bool IncludeCursor)
        {
            _monitor = Monitor;
            _includeCursor = IncludeCursor;
            _rectangle = Rectangle;

            Width = Rectangle.Width;
            Height = Rectangle.Height;

            Reinit();
        }

        void Reinit()
        {
            _dupl?.Dispose();

            _dupl = new DesktopDuplicator(_rectangle, _includeCursor, _monitor)
            {
                Timeout = Timeout
            };
        }
        
        public int Height { get; }

        public int Width { get; }
        
        public IBitmapFrame Capture()
        {
            try
            {
                return _dupl.Capture();
            }
            catch
            {
                try { Reinit(); }
                catch
                {
                    return RepeatFrame.Instance;
                }

                try
                {
                    return _dupl.Capture();
                }
                catch
                {
                    return new OneTimeFrame(new Bitmap(Width, Height));
                }
            }
        }

        public void Dispose()
        {
            _dupl?.Dispose();
        }
    }
}
