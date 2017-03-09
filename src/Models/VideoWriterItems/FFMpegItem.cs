using Screna;
using Screna.Audio;

namespace Captura
{
    public delegate void FFMpegArgsProvider(out string AudioCofig, out string VideoConfig);

    public class FFMpegItem : IVideoWriterItem
    {
        public static FFMpegItem[] Items { get; } =
        {
            // MP4 (x264, AAC)
            new FFMpegItem("Mp4 (x264 | AAC)", ".mp4", (out string AudioConfig, out string VideoConfig) =>
            {
                VideoConfig = "-vcodec libx264 -pix_fmt yuv420p";

                AudioConfig = "-acodec aac -b:a 192k";
            }),

            // Avi
            new FFMpegItem("Avi", ".avi", (out string AudioConfig, out string VideoConfig) =>
            {
                AudioConfig = "";

                VideoConfig = "";
            })
        };

        FFMpegItem(string Name, string Extension, FFMpegArgsProvider ArgsProvider)
        {
            this.ArgsProvider = ArgsProvider;
            this.Extension = Extension;
            _name = Name;
        }

        public string Extension { get; }

        readonly string _name;
        public override string ToString() => _name;

        public IVideoFileWriter GetVideoFileWriter(string FileName, int FrameRate, IImageProvider ImageProvider, IAudioProvider AudioProvider)
        {
            if (AudioProvider == null)
                return new FFMpegVideoWriter(FileName, FrameRate, this);

            return new FFMpegMuxedWriter(FileName, FrameRate, this, AudioProvider);
        }

        public FFMpegArgsProvider ArgsProvider { get; }
    }
}
