using Screna;
using Screna.Audio;

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

        public IVideoFileWriter GetVideoFileWriter(string FileName, int FrameRate, int VideoQuality, IImageProvider ImageProvider, int AudioQuality, IAudioProvider AudioProvider)
        {
            _codec.Quality = VideoQuality;

            return new AviWriter(FileName, _codec, ImageProvider, FrameRate, AudioProvider);
        }
        
        public override string ToString() => _codec.Name;
    }
}
