using Screna;
using System;
using System.Drawing;

namespace Captura
{
    public interface IVSLI
    {
        IImageProvider GetImageProvider(out Func<Point> OverlayOffset);
    }
}
