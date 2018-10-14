using System;
using Screna;
using System.Drawing;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class FullScreenItem : NotifyPropertyChanged, IVideoItem
    {
        public override string ToString() => Name;

        public string Name => null;

        public IImageProvider GetImageProvider(bool IncludeCursor, out Func<Point, Point> Transform)
        {
			var region = WindowProvider.DesktopRectangle;

			Transform = P => new Point(P.X - region.X, P.Y - region.Y);

            return new RegionProvider(region, IncludeCursor);
		}
    }
}