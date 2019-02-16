using System;
using System.Drawing;

namespace Captura.Models
{
    /// <summary>
    /// Holds codecs for audio-alone capture.
    /// </summary>
    public class NoVideoItem : IVideoItem
    {
        public IAudioWriterItem AudioWriterItem { get; }

        public NoVideoItem(IAudioWriterItem AudioWriterItem)
        {
            this.AudioWriterItem = AudioWriterItem;
        }

        public string Name => AudioWriterItem.Name;

        public IImageProvider GetImageProvider(bool IncludeCursor, out Func<Point, Point> Transform)
        {
            Transform = null;

            return null;
        }

        public override string ToString() => Name;

        public string Extension => AudioWriterItem.Extension;
    }
}
