using DesktopDuplication;
using Screna;
using System.Drawing;

namespace Captura
{
    class StaticRegionProvider : IImageProvider
    {
        readonly RegionSelector _selector;
        DesktopDuplicator _dupl;

        public StaticRegionProvider(RegionSelector RegionSelector)
        {
            _selector = RegionSelector;

            var rect = _selector.SelectedRegion;
            Height = rect.Height;
            Width = rect.Width;
        }
        
        //public Bitmap Capture() => ScreenShot.Capture(_selector.SelectedRegion);

        public Bitmap Capture()
        {
            try
            {
                return _dupl.GetLatestFrame(_selector.SelectedRegion);
            }
            catch
            {
                _dupl = new DesktopDuplicator(0);

                return _dupl.GetLatestFrame(_selector.SelectedRegion);
            }
        }

        public int Height { get; }

        public int Width { get; }
        
        public void Dispose()
        {
            _dupl?.Dispose();
        }
    }
}
