namespace Captura.Video
{
    public class StepsVideoWriterItem : IVideoWriterItem
    {
        IVideoWriterItem _writerItem;

        public StepsVideoWriterItem(IVideoWriterItem WriterItem)
        {
            _writerItem = WriterItem;
        }

        public string Extension => _writerItem.Extension;

        public IVideoFileWriter GetVideoFileWriter(VideoWriterArgs Args)
        {
            Args.FrameRate = 1;

            return _writerItem.GetVideoFileWriter(Args);
        }

        public override string ToString() => "Video";

        public string Description => "Writes as a Video.";
    }
}