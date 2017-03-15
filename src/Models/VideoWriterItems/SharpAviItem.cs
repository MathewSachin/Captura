using Screna;
using Screna.Audio;
using Screna.Avi;

namespace Captura
{
    public class SharpAviItem : IVideoWriterItem
    {
        readonly AviCodec _codec;

        public SharpAviItem(AviCodec Codec)
        {
            _codec = Codec;
        }

        public string Extension { get; } = ".avi";

        public IVideoFileWriter GetVideoFileWriter(string FileName, int FrameRate, int Quality, IImageProvider ImageProvider, IAudioProvider AudioProvider)
        {
            _codec.Quality = Quality;

            return new AviWriter(FileName, _codec, ImageProvider, FrameRate, AudioProvider);
        }
        
        public override string ToString() => _codec.Name;
    }
}
