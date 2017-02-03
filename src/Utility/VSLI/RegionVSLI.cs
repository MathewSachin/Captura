using Screna;

namespace Captura
{
    class RegionVSLI : IVSLI
    {
        public IImageProvider GetImageProvider(params IOverlay[] Overlays)
        {
            return new StaticRegionProvider(RegionSelector.Instance, Overlays);
        }

        public override string ToString() => "RegionSelector";
    }
}
