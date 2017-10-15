using Screna;
using System.Drawing;

namespace Captura
{
    class StaticRegionProvider : IImageProvider
    {
        readonly RegionSelector _selector;
        readonly RegionProvider _regionProvider;

        public StaticRegionProvider(RegionSelector RegionSelector, bool IncludeCursor)
        {
            _selector = RegionSelector;

            var rect = _selector.SelectedRegion.Even();
            Height = rect.Height;
            Width = rect.Width;

            _regionProvider = new RegionProvider(rect, IncludeCursor);
        }
        
        public Bitmap Capture()
        {
            _regionProvider.UpdateLocation(_selector.SelectedRegion.Location);

            return _regionProvider.Capture();
        }

        public int Height { get; }

        public int Width { get; }

        public void Dispose()
        {
            _regionProvider.Dispose();
        }
    }
}
