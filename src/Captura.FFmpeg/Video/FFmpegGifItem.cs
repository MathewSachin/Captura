namespace Captura.Models
{
    // ReSharper disable once InconsistentNaming
    class FFmpegGifItem : IVideoWriterItem
    {
        public string Extension { get; } = "gif";

        public IVideoFileWriter GetVideoFileWriter(VideoWriterArgs Args)
        {
            return new FFmpegGifWriter(Args);
        }

        public override string ToString() => "Gif (Post Processing)";

        public string Description => "Encoding is done after recording has been finished.";
    }
}