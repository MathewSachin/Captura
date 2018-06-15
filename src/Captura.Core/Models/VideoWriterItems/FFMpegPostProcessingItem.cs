using System.Collections.Generic;
using Screna;

namespace Captura.Models
{
    // ReSharper disable once InconsistentNaming
    public class FFMpegPostProcessingItem : IVideoWriterItem
    {
        readonly string _name;
        readonly FFMpegVideoArgsProvider _videoArgsProvider;
        readonly FFMpegAudioArgsProvider _audioArgsProvider;

        public FFMpegPostProcessingItem(string Name, string Extension, FFMpegVideoArgsProvider VideoArgsProvider, FFMpegAudioArgsProvider AudioArgsProvider)
        {
            _name = Name;
            _videoArgsProvider = VideoArgsProvider;
            _audioArgsProvider = AudioArgsProvider;
            this.Extension = Extension;
        }

        public string Extension { get; }

        public IVideoFileWriter GetVideoFileWriter(VideoWriterArgs Args)
        {
            return new FFMpegPostProcessingWriter(FFMpegVideoWriterArgs.FromVideoWriterArgs(Args, _videoArgsProvider, _audioArgsProvider));
        }

        public override string ToString() => _name + " (Post Processing)";

        public static IEnumerable<FFMpegPostProcessingItem> Items { get; } = new[]
        {
            new FFMpegPostProcessingItem("WebM (Vp8 | Opus)", ".webm", VideoQuality =>
            {
                // quality: 63 (lowest) to 4 (highest)
                var crf = 63 - ((VideoQuality - 1) * 59) / 99;

                return $"-vcodec libvpx -crf {crf} -b:v 1M";
            }, FFMpegAudioItem.Opus),

            new FFMpegPostProcessingItem("WebM (Vp9 | Opus)", ".webm", VideoQuality =>
            {
                // quality: 63 (lowest) to 0 (highest)
                var crf = (63 * (100 - VideoQuality)) / 99;

                return $"-vcodec libvpx-vp9 -crf {crf} -b:v 0";
            }, FFMpegAudioItem.Opus)
        };
    }
}