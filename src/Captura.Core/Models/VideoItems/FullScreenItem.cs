using System;
using Screna;
using System.Drawing;
using Captura.Properties;

namespace Captura.Models
{
    public class FullScreenItem : IVideoItem
    {
        public static FullScreenItem Instance { get; } = new FullScreenItem();

        FullScreenItem() { }
                
        public override string ToString() => Resources.FullScreen;

        public IImageProvider GetImageProvider(out Func<Point> Offset)
        {
            Offset = () => Point.Empty;

            return new RegionProvider(WindowProvider.DesktopRectangle);
        }
    }
}