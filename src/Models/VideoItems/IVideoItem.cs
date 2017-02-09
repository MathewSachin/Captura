using Screna;
using System;
using System.Drawing;

namespace Captura
{
    public interface IVideoItem
    {
        IImageProvider GetImageProvider(out Func<Point> OverlayOffset);
    }
}
