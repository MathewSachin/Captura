using Screna;
using Screna.Audio;

namespace Captura.Models
{
    public class FFMpegItem : IVideoWriterItem
    {
        public static FFMpegVideoArgsProvider x264 { get; } = VideoQuality =>
        {
            // quality: 51 (lowest) to 0 (highest)
            var crf = (51 * (100 - VideoQuality)) / 99;

            return $"-vcodec libx264 -crf {crf} -pix_fmt yuv420p -preset ultrafast";
        };
        
        public static FFMpegVideoArgsProvider Avi { get; } = VideoQuality =>
        {
            // quality: 31 (lowest) to 1 (highest)
            var qscale = 31 - ((VideoQuality - 1) * 30) / 99;

            return $"-vcodec libxvid -qscale:v {qscale}";
        };

        public static FFMpegVideoArgsProvider Gif { get; } = VideoQuality =>
        {
            return "";
        };

        public static FFMpegVideoArgsProvider HEVC_QSV { get; } = VideoQuality =>
        {
            return $"-vcodec hevc_qsv -load_plugin hevc_hw -q 2 -preset:v veryfast";
        };

        public static FFMpegItem[] Items { get; } =
        {
            // MP4 (x264, AAC)
            new FFMpegItem("Mp4 (x264 | AAC)", ".mp4", x264, FFMpegAudioItem.Aac),

            // Avi (Xvid, Mp3)
            new FFMpegItem("Avi (Xvid | Mp3)", ".avi", Avi, FFMpegAudioItem.Mp3),

            // Gif (No Audio)
            new FFMpegItem("Gif (No Audio)", ".gif", Gif, FFMpegAudioItem.Mp3),

            // MP4 (HEVC Intel QSV, AAC)
            new FFMpegItem("Mp4 (HEVC Intel QSV | AAC) (Skylake or above)", ".mp4", HEVC_QSV, FFMpegAudioItem.Aac)
        };
        
        FFMpegItem(string Name, string Extension, FFMpegVideoArgsProvider VideoArgsProvider, FFMpegAudioArgsProvider AudioArgsProvider)
        {            
            this.Extension = Extension;
            _videoArgsProvider = VideoArgsProvider;
            _audioArgsProvider = AudioArgsProvider;
            _name = Name;
        }

        public string Extension { get; }

        readonly string _name;
        readonly FFMpegVideoArgsProvider _videoArgsProvider;
        readonly FFMpegAudioArgsProvider _audioArgsProvider;
        public override string ToString() => _name;

        public IVideoFileWriter GetVideoFileWriter(string FileName, int FrameRate, int VideoQuality, IImageProvider ImageProvider, int AudioQuality, IAudioProvider AudioProvider)
        {
            return new FFMpegWriter(FileName, ImageProvider, FrameRate, VideoQuality, _videoArgsProvider, AudioQuality, _audioArgsProvider, AudioProvider);
        }        
    }
}
