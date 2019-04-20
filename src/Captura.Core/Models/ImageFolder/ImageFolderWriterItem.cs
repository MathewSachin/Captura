namespace Captura.Models
{
    public class ImageFolderWriterItem : IVideoWriterItem
    {
        public string Extension { get; } = "";

        public IVideoFileWriter GetVideoFileWriter(VideoWriterArgs Args)
        {
            return new ImageFolderWriter(Args.FileName);
        }

        public override string ToString() => "Preview";

        public string Description => "For testing purposes.";
    }
}