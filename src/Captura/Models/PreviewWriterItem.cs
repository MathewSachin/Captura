using Screna;
using Screna.Audio;

namespace Captura.Models
{
    public class PreviewWriterItem : IVideoWriterItem
    {
        public string Extension { get; } = "";

        public IVideoFileWriter GetVideoFileWriter(string FileName, int FrameRate, int VideoQuality, IImageProvider ImageProvider,
            int AudioQuality, IAudioProvider AudioProvider)
        {
            return new PreviewWriter();
        }

        public override string ToString() => "Preview";
    }
}