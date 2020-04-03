namespace Captura.Video
{
    public class ImageFolderWriterItem : IVideoWriterItem
    {
        public string Extension { get; } = "";

        public IVideoFileWriter GetVideoFileWriter(VideoWriterArgs Args)
        {
            return new ImageFolderWriter(Args.FileName);
        }

        public override string ToString() => "Images";

        public string Description => "Writes images to a folder.";
    }
}