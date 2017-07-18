using Captura.Models;
using Captura.Properties;
using Screna;
using System;
using System.Drawing;

namespace Captura
{
    class RegionItem : IVideoItem
    {
        readonly RegionSelector _selector;

        public RegionItem(RegionSelector RegionSelector)
        {
            _selector = RegionSelector;
        }

        public IImageProvider GetImageProvider(bool IncludeCursor, out Func<Point> Offset)
        {
            Offset = () => _selector.SelectedRegion.Location;

            return new StaticRegionProvider(_selector, IncludeCursor);
        }

        public override string ToString() => Resources.RegionSelector;
    }
}
