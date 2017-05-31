using Captura.Models;
using Captura.Properties;
using Screna;
using System;
using System.Drawing;

namespace Captura
{
    class RegionItem : IVideoItem
    {
        // Singleton
        public static RegionItem Instance { get; } = new RegionItem();

        RegionItem() { }

        public IImageProvider GetImageProvider(out Func<Point> Offset)
        {
            Offset = () => RegionSelector.Instance.Rectangle.Location;

            return new StaticRegionProvider();
        }

        public override string ToString() => Resources.RegionSelector;
    }
}
