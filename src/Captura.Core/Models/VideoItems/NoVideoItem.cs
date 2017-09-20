using Screna;
using Screna.Audio;
using System;
using System.Drawing;

namespace Captura.Models
{
    /// <summary>
    /// Holds codecs for audio-alone capture.
    /// </summary>
    public abstract class NoVideoItem : NotifyPropertyChanged, IVideoItem
    {
        protected NoVideoItem(string DisplayName, string Extension)
        {
            Name = DisplayName;

            this.Extension = Extension;
        }

        public string Name { get; }

        public IImageProvider GetImageProvider(bool IncludeCursor, out Func<Point, Point> Transform)
        {
            Transform = null;

            return null;
        }

        public override string ToString() => Name;

        public string Extension { get; }

        public abstract IAudioFileWriter GetAudioFileWriter(string FileName, WaveFormat Wf, int AudioQuality);
    }
}
