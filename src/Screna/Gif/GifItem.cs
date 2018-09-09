using Screna;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class GifItem : IVideoWriterItem
    {
        readonly GifSettings _settings;

        public GifItem(GifSettings Settings)
        {
            _settings = Settings;
        }

        public string Extension { get; } = ".gif";

        public string Description => "Encode GIF";

        public override string ToString() => "Gif";

        public IVideoFileWriter GetVideoFileWriter(VideoWriterArgs Args)
        {
            var repeat = _settings.Repeat ? _settings.RepeatCount : -1;
            
            return new GifWriter(Args.FileName, Args.FrameRate, repeat);
        }
    }
}
