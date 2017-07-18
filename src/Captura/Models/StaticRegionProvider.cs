using DesktopDuplication;
using Screna;
using System.Drawing;

namespace Captura
{
    class StaticRegionProvider : IImageProvider
    {
        readonly RegionSelector _selector;
        DesktopDuplicator _dupl;
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
            var region = _selector.SelectedRegion.Even();

            if (Settings.Instance.UseDeskDupl)
            {
                try
                {
                    _dupl.UpdateRectLocation(region.Location);

                    return _dupl.GetLatestFrame();
                }
                catch
                {
                    try
                    {
                        _dupl?.Dispose();

                        _dupl = new DesktopDuplicator(region, _includeCursor, 0);

                        return _dupl.GetLatestFrame();
                    }
                    catch
                    {
                        return new Bitmap(Width, Height);
                    }
                }
            }
            else return ScreenShot.Capture(region, _includeCursor);
        }

        public int Height { get; }

        public int Width { get; }
        
        public void Dispose()
        {
            _dupl?.Dispose();
        }
    }
}
