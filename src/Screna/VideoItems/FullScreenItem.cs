using System;
using System.Drawing;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    class FullScreenItem : NotifyPropertyChanged, IVideoItem
    {
        readonly IPlatformServices _platformServices;

        public FullScreenItem(IPlatformServices PlatformServices)
        {
            _platformServices = PlatformServices;
        }

        public override string ToString() => Name;

        public string Name => null;

        public IImageProvider GetImageProvider(bool IncludeCursor, out Func<Point, Point> Transform)
        {
			var region = _platformServices.DesktopRectangle;

			Transform = P => new Point(P.X - region.X, P.Y - region.Y);

            return _platformServices.GetRegionProvider(region, IncludeCursor);
		}
    }
}