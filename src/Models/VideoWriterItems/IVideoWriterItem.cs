using Screna;
using Screna.Audio;

namespace Captura
{
    public interface IVideoWriterItem
    {
        string Extension { get; }

        IVideoFileWriter GetVideoFileWriter(string FileName, int FrameRate, IImageProvider ImageProvider, IAudioProvider AudioProvider);
    }
}
