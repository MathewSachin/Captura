using Screna;
using Screna.Audio;

namespace Captura
{
    /// <summary>
    /// Items to show in Video Writers list.
    /// </summary>
    public interface IVideoWriterItem
    {
        string Extension { get; }

        IVideoFileWriter GetVideoFileWriter(string FileName, int FrameRate, IImageProvider ImageProvider, IAudioProvider AudioProvider);
    }
}
