using Screna;

namespace Captura.Models
{
    public class GifItem : IVideoWriterItem
    {
        public string Extension { get; } = ".gif";

        public override string ToString() => "Gif";

        public IVideoFileWriter GetVideoFileWriter(VideoWriterArgs Args)
        {
            var settings = ServiceProvider.Get<Settings>();

            var repeat = settings.Gif.Repeat ? settings.Gif.RepeatCount : -1;
            
            return new GifWriter(Args.FileName, Args.FrameRate, repeat);
        }
    }
}
