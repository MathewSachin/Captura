namespace Captura.Video
{
    /// <summary>
    /// Items to show in Video Writers list.
    /// </summary>
    public interface IVideoWriterItem
    {
        // file extension including the leading dot
        string Extension { get; }

        string Description { get; }

        IVideoFileWriter GetVideoFileWriter(VideoWriterArgs Args);
    }
}
