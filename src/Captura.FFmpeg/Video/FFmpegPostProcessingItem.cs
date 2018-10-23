using System.Collections.Generic;

namespace Captura.Models
{
    // ReSharper disable once InconsistentNaming
    public class FFmpegPostProcessingItem : IVideoWriterItem
    {
        readonly string _name;
        readonly FFmpegVideoArgsProvider _videoArgsProvider;
        readonly FFmpegAudioArgsProvider _audioArgsProvider;

        public FFmpegPostProcessingItem(string Name, string Extension, FFmpegVideoArgsProvider VideoArgsProvider, FFmpegAudioArgsProvider AudioArgsProvider)
        {
            _name = Name;
            _videoArgsProvider = VideoArgsProvider;
            _audioArgsProvider = AudioArgsProvider;
            this.Extension = Extension;
        }

        public string Extension { get; }

        public IVideoFileWriter GetVideoFileWriter(VideoWriterArgs Args)
        {
            return new FFmpegPostProcessingWriter(FFmpegVideoWriterArgs.FromVideoWriterArgs(Args, _videoArgsProvider, _audioArgsProvider));
        }

        public override string ToString() => _name + " (Post Processing)";

        public string Description => "Encoding is done after recording has been finished.";

        public static IEnumerable<FFmpegPostProcessingItem> Items { get; } = new[]
        {
            new FFmpegPostProcessingItem("WebM (Vp8 | Opus)", ".webm", VideoQuality =>
            {
                // quality: 63 (lowest) to 4 (highest)
                var crf = 63 - ((VideoQuality - 1) * 59) / 99;

                return $"-vcodec libvpx -crf {crf} -b:v 1M";
            }, FFmpegAudioItem.Opus),

            new FFmpegPostProcessingItem("WebM (Vp9 | Opus)", ".webm", VideoQuality =>
            {
                // quality: 63 (lowest) to 0 (highest)
                var crf = (63 * (100 - VideoQuality)) / 99;

                return $"-vcodec libvpx-vp9 -crf {crf} -b:v 0";
            }, FFmpegAudioItem.Opus)
        };
    }
}