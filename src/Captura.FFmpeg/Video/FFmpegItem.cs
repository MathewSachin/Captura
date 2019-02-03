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
        public static FFmpegItem x264 { get; } = new FFmpegItem(
            "Mp4 (H.264, AAC)",
            () => ".mp4",
            VideoQuality =>
            {
                var settings = ServiceProvider.Get<FFmpegSettings>().X264;

                // quality: 51 (lowest) to 0 (highest)
                var crf = (51 * (100 - VideoQuality)) / 99;

                return $"-vcodec libx264 -crf {crf} -pix_fmt {settings.PixelFormat} -preset {settings.Preset}";
            },
            FFmpegAudioItem.Aac,
            "Encode to Mp4: H.264 with AAC audio using x264 encoder.");

        // Avi (Xvid, Mp3)
        public static FFmpegItem Avi { get; } = new FFmpegItem(
            "Avi (Xvid, Mp3)",
            () => ".avi",
            VideoQuality =>
            {
                // quality: 31 (lowest) to 1 (highest)
                var qscale = 31 - ((VideoQuality - 1) * 30) / 99;

                return $"-vcodec libxvid -qscale:v {qscale}";
            },
            FFmpegAudioItem.Mp3,
            "Encode to Avi with Mp3 audio using Xvid encoder");

        // Gif (No Audio)
        public static FFmpegItem Gif { get; } = new FFmpegItem(
            "Gif (No Audio)",
            () => ".gif",
            VideoQuality => "",
            FFmpegAudioItem.Mp3);

        // MP4 (HEVC Intel QSV, AAC)
        public static FFmpegItem HEVC_QSV { get; } = new FFmpegItem(
            "Intel QuickSync: Mp4 (HEVC, AAC)",
            () => ".mp4",
            VideoQuality => "-vcodec hevc_qsv -load_plugin hevc_hw -q 2 -preset:v veryfast",
            FFmpegAudioItem.Aac,
            "Encode to Mp4: HEVC (H.265) with AAC audio using Intel QuickSync hardware encoding.\nRequires the processor to be Skylake generation or later");

        const string NVencSupport = "If this doesn't work, please check on NVIDIA's website whether your graphic card supports NVenc.";

        // MP4 (H.264 NVENC, AAC)
        public static FFmpegItem H264_NVENC { get; } = new FFmpegItem(
            "NVenc: Mp4 (H.264, AAC)",
            () => ".mp4",
            VideoQuality => "-c:v h264_nvenc -pixel_format yuv444p -preset fast",
            FFmpegAudioItem.Aac,
            NVencSupport);

        // MP4 (HEVC NVENC, AAC)
        public static FFmpegItem HEVC_NVENC { get; } = new FFmpegItem(
            "NVenc: Mp4 (HEVC, AAC)",
            () => ".mp4",
            VideoQuality => "-c:v hevc_nvenc -pixel_format yuv444p -preset fast",
            FFmpegAudioItem.Aac,
            NVencSupport);

        public static IEnumerable<FFmpegItem> Encoders { get; } = new[]
        {
            x264,
            Avi,
            // Gif
        };

        public static IEnumerable<FFmpegItem> HardwareEncoders { get; } = new[]
        {
            HEVC_QSV,
            H264_NVENC,
            HEVC_NVENC
        };

        public string Description { get; }
        
        FFmpegItem(string Name, Func<string> Extension, FFmpegVideoArgsProvider VideoArgsProvider, FFmpegAudioArgsProvider AudioArgsProvider, string Description = "")
            : this(Name, Extension, Description)
        {
            _videoArgsProvider = VideoArgsProvider;
            _audioArgsProvider = AudioArgsProvider;
        }

        protected FFmpegItem(string Name, Func<string> Extension, string Description)
        {
            _name = Name;
            _extension = Extension;
            this.Description = Description;
        }

        public FFmpegItem(CustomFFmpegCodec CustomCodec) : this(CustomCodec.Name,
            () => CustomCodec.Extension, "Custom Codec")
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
