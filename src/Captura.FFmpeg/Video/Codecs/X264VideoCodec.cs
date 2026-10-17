using System;
using Captura.Video;

namespace Captura.FFmpeg
{
    class X264VideoCodec : FFmpegVideoCodec
    {
        const string Descr = "Encode to Mp4: H.264 with AAC audio using x264 encoder.";

        public X264VideoCodec() : base("Mp4 (H.264, AAC)", ".mp4", Descr) { }

        public override FFmpegAudioArgsProvider AudioArgsProvider => FFmpegAudioItem.Aac;

        public override void Apply(FFmpegSettings Settings, VideoWriterArgs WriterArgs, FFmpegOutputArgs OutputArgs)
        {
            // quality: 51 (lowest) to 1 (highest)
            // CRF 0 is lossless in High 4:4:4 Predictive, unsupported by Windows' built-in decoder
            var crf = Math.Max(1, (51 * (100 - WriterArgs.VideoQuality)) / 99);

            OutputArgs.AddArg("vcodec", "libx264")
                .AddArg("crf", crf)
                .AddArg("pix_fmt", Settings.X264.PixelFormat)
                .AddArg("preset", Settings.X264.Preset);
        }
    }
}