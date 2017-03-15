using Screna;
using Screna.Audio;

namespace Captura
{
    public class FFMpegItem : IVideoWriterItem
    {
        public static FFMpegItem[] Items { get; } =
        {
            // MP4 (x264, AAC)
            new FFMpegItem("Mp4 (x264 | AAC)", ".mp4", (int VideoQuality, out string AudioConfig, out string VideoConfig) =>
            {
                // quality: 51 (lowest) to 0 (highest)
                var crf = (51 * (100 - VideoQuality)) / 99;

                VideoConfig = $"-vcodec libx264 -crf {crf} -pix_fmt yuv420p";

                AudioConfig = "-acodec aac -strict -2 -ac 2 -b:a 192k";
            }),

            // Avi
            new FFMpegItem("Avi (Xvid)", ".avi", (int VideoQuality, out string AudioConfig, out string VideoConfig) =>
            {
                // quality: 31 (lowest) to 1 (highest)
                var qscale = 31 - ((VideoQuality - 1) * 30) / 99;

                VideoConfig = $"-vcodec libxvid -qscale:v {qscale}";

                AudioConfig = "";
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

        public IVideoFileWriter GetVideoFileWriter(string FileName, int FrameRate, int Quality, IImageProvider ImageProvider, IAudioProvider AudioProvider)
        {
            if (AudioProvider == null)
                return new FFMpegVideoWriter(FileName, FrameRate, Quality, this);

            return new FFMpegMuxedWriter(FileName, FrameRate, Quality, this, AudioProvider);
        }

        public FFMpegArgsProvider ArgsProvider { get; }
    }
}
