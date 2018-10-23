using System;
using System.ComponentModel;
using System.Drawing;

namespace Captura.Models
{
    /// <summary>
    /// Items to show in Video Source List.
    /// </summary>
    public interface IVideoItem : INotifyPropertyChanged
    {
        string Name { get; }

        IImageProvider GetImageProvider(bool IncludeCursor, out Func<Point, Point> Transform);
    }
}
