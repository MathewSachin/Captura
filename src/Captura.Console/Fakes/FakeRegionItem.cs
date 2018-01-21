using System;
using System.Drawing;
using Captura.Models;
using Screna;

namespace Captura.Console
{
    class FakeRegionItem : NotifyPropertyChanged, IVideoItem
    {
        Rectangle _rect;

        public FakeRegionItem(Rectangle Region)
        {
            _rect = Region;
        }

        public string Name => LanguageManager.Instance.RegionSelector;

        public IImageProvider GetImageProvider(bool IncludeCursor, out Func<Point, Point> Transform)
        {
            Transform = P => new Point(P.X - _rect.X, P.Y - _rect.Y);
            
            return new RegionProvider(_rect, IncludeCursor);
        }

        public override string ToString() => Name;
    }
}
