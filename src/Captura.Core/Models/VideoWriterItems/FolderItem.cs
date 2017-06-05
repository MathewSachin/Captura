using Screna;
using Screna.Audio;

namespace Captura.Models
{
    public class FolderItem : IVideoWriterItem
    {
        public string Extension => "";

        public IVideoFileWriter GetVideoFileWriter(string FileName, int FrameRate, int VideoQuality, IImageProvider ImageProvider, int AudioQuality, IAudioProvider AudioProvider)
        {
            return new FolderWriter(FileName, FrameRate, AudioProvider);
        }

        public override string ToString() => "Folder";
    }
}
