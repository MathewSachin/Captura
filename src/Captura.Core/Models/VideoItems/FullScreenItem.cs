using System;
using Screna;
using System.Drawing;
using Captura.Properties;

namespace Captura.Models
{
    public class FullScreenItem : IVideoItem
    {
        public static FullScreenItem Instance { get; } = new FullScreenItem();

        Window _desktop;

        FullScreenItem()
        {
            _desktop = Window.DesktopWindow;
        }
                
        public override string ToString() => Resources.FullScreen;

        public IImageProvider GetImageProvider(out Func<Point> Offset)
        {
            Offset = () => Point.Empty;
            
            return new WindowProvider(() => _desktop, Settings.Instance.VideoBackgroundColor);
        }
    }
}