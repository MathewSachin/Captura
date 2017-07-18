using Screna;
using Screna.Audio;
using System;
using System.Drawing;

namespace Captura.Models
{
    /// <summary>
    /// Holds codecs for audio-alone capture.
    /// </summary>
    public abstract class NoVideoItem : IVideoItem
    {
        string _name;

        public NoVideoItem(string DisplayName, string Extension)
        {
            _name = DisplayName;

            this.Extension = Extension;
        }

        public IImageProvider GetImageProvider(bool IncludeCursor, out Func<Point> OverlayOffset)
        {
            OverlayOffset = null;

            return null;
        }

        public override string ToString() => _name;

        public string Extension { get; }

        public abstract IAudioFileWriter GetAudioFileWriter(string FileName, WaveFormat Wf, int AudioQuality);
    }
}
