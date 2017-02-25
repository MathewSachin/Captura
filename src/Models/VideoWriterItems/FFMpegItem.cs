using Screna;
using Screna.Audio;

namespace Captura
{
    public class FFMpegItem : IVideoWriterItem
    {
        static FFMpegItem _instance;

        public static FFMpegItem Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new FFMpegItem();

                return _instance;
            }
        }

        FFMpegItem() { }

        public string Extension { get; } = ".mp4";

        public override string ToString() => "Mp4 (x264 | AAC)";

        public IVideoFileWriter GetVideoFileWriter(string FileName, int FrameRate, IImageProvider ImageProvider, IAudioProvider AudioProvider)
        {
            return new FFMpegVideoWriter(FileName, FrameRate, AudioProvider);
        }
    }
}
