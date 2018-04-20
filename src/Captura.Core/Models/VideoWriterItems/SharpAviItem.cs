using Screna;

namespace Captura.Models
{
    public class SharpAviItem : IVideoWriterItem
    {
        readonly AviCodec _codec;

        public SharpAviItem(AviCodec Codec)
        {
            _codec = Codec;
        }

        public string Extension { get; } = ".avi";

        public IVideoFileWriter GetVideoFileWriter(VideoWriterArgs Args)
        {
            _codec.Quality = Args.VideoQuality;

            return new AviWriter(Args.FileName, _codec, Args.ImageProvider, Args.FrameRate, Args.AudioProvider);
        }
        
        public override string ToString() => _codec.Name;
    }
}
