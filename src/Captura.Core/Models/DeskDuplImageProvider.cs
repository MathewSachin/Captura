using System.Drawing;
using Screna;
using DesktopDuplication;

namespace Captura.Models
{
    public class DeskDuplImageProvider : IImageProvider
    {
        DesktopDuplicator _dupl;
        int _monitor;

        public DeskDuplImageProvider(int Monitor = 0)
        {
            _monitor = Monitor;

            Width = WindowProvider.DesktopRectangle.Width;
            Height = WindowProvider.DesktopRectangle.Height;
        }
        
        public int Height { get; }

        public int Width { get; }

        public Bitmap Capture()
        {
            try
            {
                return _dupl.GetLatestFrame(WindowProvider.DesktopRectangle);
            }
            catch
            {
                _dupl = new DesktopDuplicator(_monitor);

                return _dupl.GetLatestFrame(WindowProvider.DesktopRectangle);
            }
        }

        public void Dispose()
        {
            _dupl?.Dispose();
        }
    }
}
