using Captura.Video;

namespace Captura.SharpAvi
{
    class SharpAviItem : IVideoWriterItem
    {
        readonly AviCodec _codec;

        public SharpAviItem(AviCodec Codec, string Description)
        {
            _codec = Codec;
            this.Description = Description;
        }

        public string Extension { get; } = ".avi";

        public string Description { get; }

        public IVideoFileWriter GetVideoFileWriter(VideoWriterArgs Args)
        {
            _codec.Quality = Args.VideoQuality;

            return new AviWriter(Args.FileName, _codec, Args.ImageProvider, Args.FrameRate, Args.AudioProvider);
        }
        
        public override string ToString() => _codec.Name;
    }
}
