using Captura.Models;
using Screna;
using System;
using System.Drawing;

namespace Captura
{
    class RegionItem : NotifyPropertyChanged, IVideoItem
    {
        readonly RegionSelector _selector;

        public RegionItem(RegionSelector RegionSelector)
        {
            _selector = RegionSelector;

            LanguageManager.Instance.LanguageChanged += L => RaisePropertyChanged(nameof(Name));
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

        public string Name => LanguageManager.Instance.RegionSelector;

        public override string ToString() => Name;
    }
}
