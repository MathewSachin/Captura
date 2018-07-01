using System;
using System.Collections.Generic;
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable InconsistentNaming
// ReSharper disable ArrangeRedundantParentheses

namespace Captura.Models
{
    public class FFmpegItem : IVideoWriterItem
    {
        // MP4 (x264, AAC)
        public static FFmpegItem x264 { get; } = new FFmpegItem("Mp4 (x264 | AAC)", () => ".mp4", VideoQuality =>
        {
            var settings = ServiceProvider.Get<FFmpegSettings>().X264;

            // quality: 51 (lowest) to 0 (highest)
            var crf = (51 * (100 - VideoQuality)) / 99;

            return $"-vcodec libx264 -crf {crf} -pix_fmt {settings.PixelFormat} -preset {settings.Preset}";
        }, FFmpegAudioItem.Aac);

        // Avi (Xvid, Mp3)
        public static FFmpegItem Avi { get; } = new FFmpegItem("Avi (Xvid | Mp3)", () => ".avi", VideoQuality =>
        {
            // quality: 31 (lowest) to 1 (highest)
            var qscale = 31 - ((VideoQuality - 1) * 30) / 99;

            return $"-vcodec libxvid -qscale:v {qscale}";
        }, FFmpegAudioItem.Mp3);

        // Gif (No Audio)
        public static FFmpegItem Gif { get; } = new FFmpegItem("Gif (No Audio)", () => ".gif", VideoQuality => "", FFmpegAudioItem.Mp3);

        // MP4 (HEVC Intel QSV, AAC)
        public static FFmpegItem HEVC_QSV { get; } = new FFmpegItem("Mp4 (HEVC Intel QSV | AAC) (Skylake or above)", () => ".mp4",
            VideoQuality => "-vcodec hevc_qsv -load_plugin hevc_hw -q 2 -preset:v veryfast", FFmpegAudioItem.Aac);

        // MP4 (H.264 NVENC, AAC)
        public static FFmpegItem H264_NVENC { get; } = new FFmpegItem("Mp4 (H.264 NVENC | AAC) (Alpha)", () => ".mp4",
            VideoQuality => "-c:v h264_nvenc -profile:v high444p -pixel_format yuv444p -preset fast", FFmpegAudioItem.Aac);

        // MP4 (HEVC NVENC, AAC)
        public static FFmpegItem HEVC_NVENC { get; } = new FFmpegItem("Mp4 (HEVC NVENC | AAC) (Alpha)", () => ".mp4",
            VideoQuality => "-c:v hevc_nvenc -profile:v high444p -pixel_format yuv444p -preset slow", FFmpegAudioItem.Aac);

        public static IEnumerable<FFmpegItem> Items { get; } = new[]
        {
            x264,
            Avi,
            Gif,
            HEVC_QSV,
            H264_NVENC,
            HEVC_NVENC
        };
        
        FFmpegItem(string Name, Func<string> Extension, FFmpegVideoArgsProvider VideoArgsProvider, FFmpegAudioArgsProvider AudioArgsProvider)
            : this(Name, Extension)
        {
            _videoArgsProvider = VideoArgsProvider;
            _audioArgsProvider = AudioArgsProvider;
        }

        protected FFmpegItem(string Name, Func<string> Extension)
        {
            _name = Name;
            _extension = Extension;
        }

        public FFmpegItem(CustomFFmpegCodec CustomCodec) : this(CustomCodec.Name,
            () => CustomCodec.Extension)
        {
            _videoArgsProvider = VideoQuality => CustomCodec.Args;

            _audioArgsProvider = FFmpegAudioItem.Mp3;

            foreach (var audioItem in FFmpegAudioItem.Items)
            {
                if (audioItem.Name.Split(' ')[0] == CustomCodec.AudioFormat)
                {
                    _audioArgsProvider = audioItem.AudioArgsProvider;
                    break;
                }
            }
        }

        readonly Func<string> _extension;

        public string Extension => _extension?.Invoke();

        readonly string _name;
        readonly FFmpegVideoArgsProvider _videoArgsProvider;
        readonly FFmpegAudioArgsProvider _audioArgsProvider;
        public override string ToString() => _name;

        public virtual IVideoFileWriter GetVideoFileWriter(VideoWriterArgs Args)
        {
            return new FFmpegWriter(FFmpegVideoWriterArgs.FromVideoWriterArgs(Args, _videoArgsProvider, _audioArgsProvider));
        }

        public IVideoFileWriter GetVideoFileWriter(VideoWriterArgs Args, string OutputArgs)
        {
            var args = FFmpegVideoWriterArgs.FromVideoWriterArgs(Args, _videoArgsProvider, _audioArgsProvider);
            args.OutputArgs = OutputArgs;

            return new FFmpegWriter(args);
        }
    }
}
