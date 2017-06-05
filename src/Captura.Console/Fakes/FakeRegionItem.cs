using System;
using System.Drawing;
using Captura.Models;
using Screna;
using Captura.Properties;

namespace Captura.Console
{
    class FakeRegionItem : IVideoItem
    {
        Rectangle _rect;

        public FakeRegionItem(Rectangle Region)
        {
            _rect = Region;
        }

        public IImageProvider GetImageProvider(out Func<Point> OverlayOffset)
        {
            OverlayOffset = () => _rect.Location;

            return new RegionProvider(_rect);
        }

        public override string ToString() => Resources.RegionSelector;
    }
}
