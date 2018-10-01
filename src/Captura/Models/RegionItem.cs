using Captura.Models;
using System;
using System.Drawing;
using Screna;

namespace Captura
{
    class RegionItem : NotifyPropertyChanged, IVideoItem
    {
        readonly IRegionProvider _selector;

        public RegionItem(IRegionProvider RegionSelector)
        {
            _selector = RegionSelector;
        }

        public IImageProvider GetImageProvider(bool IncludeCursor, out Func<Point, Point> Transform)
        {
            Transform = P =>
            {
                var region = _selector.SelectedRegion.Location;

                return new Point(P.X - region.X, P.Y - region.Y);
            };

            return new RegionProvider(_selector.SelectedRegion, IncludeCursor,
                () => _selector.SelectedRegion.Location);
        }

        string _name;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                
                OnPropertyChanged();
            }
        }

        public override string ToString() => Name;
    }
}
