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

            var rect = _selector.SelectedRegion.Even();
            Height = rect.Height;
            Width = rect.Width;
        }
        
        public Bitmap Capture()
        {
            var region = _selector.SelectedRegion.Even();

            if (Settings.Instance.UseDeskDupl)
            {
                try
                {
                    return _dupl.GetLatestFrame(region);
                }
                catch
                {
                    _dupl = new DesktopDuplicator(0);

                    return _dupl.GetLatestFrame(region);
                }
            }
            else return ScreenShot.Capture(region);
        }

        public int Height { get; }

        public int Width { get; }
        
        public void Dispose()
        {
            _dupl?.Dispose();
        }
    }
}
