using Screna;
using System;
using System.Drawing;

namespace Captura
{
    class RegionItem : IVideoItem
    {
        public IImageProvider GetImageProvider(out Func<Point> Offset)
        {
            Offset = () => RegionSelector.Instance.Rectangle.Location;

            return new StaticRegionProvider(RegionSelector.Instance);
        }

        public override string ToString() => "RegionSelector";
    }
}
