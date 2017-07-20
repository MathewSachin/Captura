using Screna;
using System.Drawing;

namespace Captura
{
    class StaticRegionProvider : IImageProvider
    {
        readonly RegionSelector _selector;
        readonly bool _includeCursor;

        public StaticRegionProvider(RegionSelector RegionSelector, bool IncludeCursor)
        {
            _selector = RegionSelector;

            var rect = _selector.SelectedRegion.Even();
            Height = rect.Height;
            Width = rect.Width;

            _includeCursor = IncludeCursor;
        }
        
        public Bitmap Capture()
        {
            return ScreenShot.Capture(_selector.SelectedRegion.Even(), _includeCursor);
        }

        public int Height { get; }

        public int Width { get; }

        public void Dispose() { }
    }
}
