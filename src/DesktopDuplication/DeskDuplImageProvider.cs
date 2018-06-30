using System.Drawing;
using Screna;
using DesktopDuplication;
using SharpDX.DXGI;

namespace Captura.Models
{
    public class DeskDuplImageProvider : IImageProvider
    {
        DesktopDuplicator _dupl;
        readonly bool _includeCursor;
        readonly Rectangle _rectangle;
        readonly Adapter1 _adapter;
        readonly Output1 _output;

        public int Timeout { get; set; }

        internal DeskDuplImageProvider(Adapter1 Adapter, Output1 Output, Rectangle Rectangle, bool IncludeCursor)
        {
            _adapter = Adapter;
            _output = Output;

            _includeCursor = IncludeCursor;
            _rectangle = Rectangle;

            Width = Rectangle.Width;
            Height = Rectangle.Height;

            Reinit();
        }

        void Reinit()
        {
            _dupl?.Dispose();

            _dupl = new DesktopDuplicator(_rectangle, _includeCursor, _adapter, _output)
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
