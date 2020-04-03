using Captura.Audio;

namespace Captura.Video
{
    public class VideoWriterArgs
    {
        public string FileName { get; set; }
        public IImageProvider ImageProvider { get; set; }
        public int FrameRate { get; set; } = 15;
        public int VideoQuality { get; set; } = 70;
        public int AudioQuality { get; set; } = 50;
        public IAudioProvider AudioProvider { get; set; }
    }
}