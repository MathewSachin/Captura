namespace Captura.Video
{
    /// <summary>
    /// Items to show in Video Source List.
    /// </summary>
    public interface IVideoItem
    {
        string Name { get; }

        IImageProvider GetImageProvider(bool IncludeCursor);
    }
}
