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

        public IImageProvider GetImageProvider(bool IncludeCursor, out Func<Point> Offset)
        {
            Offset = () => Point.Empty;

            if (Settings.Instance.UseDeskDupl)
                return new DeskDuplImageProvider(0, IncludeCursor);

            return new RegionProvider(WindowProvider.DesktopRectangle, IncludeCursor);
        }
    }
}