using Screna;
using System;
using System.Drawing;

namespace Captura.Models
{
    /// <summary>
    /// Items to show in Video Source List.
    /// </summary>
    public interface IVideoItem
    {
        IImageProvider GetImageProvider(bool IncludeCursor, out Func<Point> OverlayOffset);
    }
}
