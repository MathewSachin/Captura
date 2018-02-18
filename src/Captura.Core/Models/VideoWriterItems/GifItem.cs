using Screna;
using Screna.Audio;

namespace Captura.Models
{
    public class GifItem : IVideoWriterItem
    {
        public string Extension { get; } = ".gif";

        public override string ToString() => "Gif";

        public IVideoFileWriter GetVideoFileWriter(string FileName, int FrameRate, int VideoQuality, IImageProvider ImageProvider, int AudioQuality, IAudioProvider AudioProvider)
        {
            var settings = ServiceProvider.Get<Settings>();

            var repeat = settings.Gif.Repeat ? settings.Gif.RepeatCount : -1;
            
            return new GifWriter(FileName, FrameRate, repeat);
        }
    }
}
