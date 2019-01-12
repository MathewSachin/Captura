using DesktopDuplication;
using SharpDX.DXGI;

namespace Captura.Models
{
    class DeskDuplImageProvider : IImageProvider
    {
        readonly DesktopDuplicator _dupl;

        public DeskDuplImageProvider(Output1 Output, bool IncludeCursor)
        {
            var bounds = Output.Description.DesktopBounds;

            Width = bounds.Right - bounds.Left;
            Height = bounds.Bottom - bounds.Top;

            _dupl = new DesktopDuplicator(IncludeCursor, Output);
        }

        public int Height { get; }

        public int Width { get; }
        
        public IEditableFrame Capture()
        {
            return _dupl.Capture();
        }

        public void Dispose()
        {
            _dupl?.Dispose();
        }
    }
}
