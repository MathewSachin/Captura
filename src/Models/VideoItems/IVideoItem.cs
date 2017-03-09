using Screna;
using System;
using System.Drawing;

namespace Captura
{
    /// <summary>
    /// Items to show in Video Source List.
    /// </summary>
    public interface IVideoItem
    {
        IImageProvider GetImageProvider(out Func<Point> OverlayOffset);
    }
}
