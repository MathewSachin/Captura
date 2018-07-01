using Captura.Models;
using System;
using System.Drawing;

namespace Captura
{
    class RegionItem : NotifyPropertyChanged, IVideoItem
    {
        readonly RegionSelector _selector;
        readonly LanguageManager _loc;

        public RegionItem(RegionSelector RegionSelector, LanguageManager Loc)
        {
            _selector = RegionSelector;
            _loc = Loc;

            Loc.LanguageChanged += L => RaisePropertyChanged(nameof(Name));
        }

        public IImageProvider GetImageProvider(bool IncludeCursor, out Func<Point, Point> Transform)
        {
            Transform = P =>
            {
                var region = _selector.SelectedRegion.Location;

                return new Point(P.X - region.X, P.Y - region.Y);
            };

            return new StaticRegionProvider(_selector, IncludeCursor);
        }

        public string Name => _loc.RegionSelector;

        public override string ToString() => Name;
    }
}
