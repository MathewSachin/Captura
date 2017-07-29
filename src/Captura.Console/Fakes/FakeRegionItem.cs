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

        public IImageProvider GetImageProvider(bool IncludeCursor, out Func<Point, Point> Transform)
        {
            Transform = P => new Point(P.X - _rect.X, P.Y - _rect.Y);
            
            return new RegionProvider(_rect, IncludeCursor);
        }

        public override string ToString() => Resources.RegionSelector;
    }
}
