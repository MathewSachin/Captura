using System;
using Captura.Video;

namespace Captura.FFmpeg
{
    class QsvH264VideoCodec : FFmpegVideoCodec
    {
        const string Descr = "Encode to Mp4: H.264 with AAC audio using Intel QuickSync hardware encoding.\nRequires an Intel GPU with QuickSync support.";

        public QsvH264VideoCodec() : base("Intel QuickSync: Mp4 (H.264, AAC)", ".mp4", Descr) { }

        public override FFmpegAudioArgsProvider AudioArgsProvider => FFmpegAudioItem.Aac;

        public override void Apply(FFmpegSettings Settings, VideoWriterArgs WriterArgs, FFmpegOutputArgs OutputArgs)
        {
            // quality: 51 (lowest) to 1 (highest), same scale as x264's CRF
            var globalQuality = Math.Max(1, (51 * (100 - WriterArgs.VideoQuality)) / 99);

            OutputArgs.AddArg("vcodec", "h264_qsv")
                .AddArg("global_quality", globalQuality)
                .AddArg("preset:v", "veryfast");
        }
    }
}