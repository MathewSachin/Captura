using Screna;
using Screna.Audio;

namespace Captura.Models
{
    public class GifItem : IVideoWriterItem
    {
        // Singleton
        public static GifItem Instance { get; } = new GifItem();

        GifItem() { }

        public string Extension { get; } = ".gif";

        public override string ToString() => "Gif";

        public IVideoFileWriter GetVideoFileWriter(string FileName, int FrameRate, int VideoQuality, IImageProvider ImageProvider, int AudioQuality, IAudioProvider AudioProvider)
        {
            var settings = ServiceProvider.Get<Settings>();

            var repeat = settings.GifRepeat ? settings.GifRepeatCount : -1;
            
            return new GifWriter(FileName, FrameRate, repeat);
        }
    }
}
