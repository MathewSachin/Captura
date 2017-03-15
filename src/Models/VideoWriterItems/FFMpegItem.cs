using Screna;
using Screna.Audio;

namespace Captura
{
    public class FFMpegItem : IVideoWriterItem
    {
        public static FFMpegItem[] Items { get; } =
        {
            // MP4 (x264, AAC)
            new FFMpegItem("Mp4 (x264 | AAC)", ".mp4", (int VideoQuality, out string VideoConfig, int AudioQuality, out string AudioConfig) =>
            {
                // quality: 51 (lowest) to 0 (highest)
                var crf = (51 * (100 - VideoQuality)) / 99;

                VideoConfig = $"-vcodec libx264 -crf {crf} -pix_fmt yuv420p";

                AudioConfig = FFMpegAudioWriterItem.Aac.AudioArgsProvider(AudioQuality);
            }),

            // Avi
            new FFMpegItem("Avi (Xvid | Mp3)", ".avi", (int VideoQuality, out string VideoConfig, int AudioQuality, out string AudioConfig) =>
            {
                // quality: 31 (lowest) to 1 (highest)
                var qscale = 31 - ((VideoQuality - 1) * 30) / 99;

                VideoConfig = $"-vcodec libxvid -qscale:v {qscale}";

                AudioConfig = FFMpegAudioWriterItem.Mp3.AudioArgsProvider(AudioQuality);
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

        public IVideoFileWriter GetVideoFileWriter(string FileName, int FrameRate, int VideoQuality, IImageProvider ImageProvider, int AudioQuality, IAudioProvider AudioProvider)
        {
            if (AudioProvider == null)
                return new FFMpegVideoWriter(FileName, FrameRate, VideoQuality, this);

            return new FFMpegMuxedWriter(FileName, FrameRate, VideoQuality, this, AudioQuality, AudioProvider);
        }

        public FFMpegArgsProvider ArgsProvider { get; }
    }
}
