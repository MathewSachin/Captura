using Captura.Video;

namespace Captura.FFmpeg
{
    class Vp8VideoCodec : FFmpegVideoCodec
    {
        const string Descr = "Encode to WebM: Vp8 with Opus audio";

        public Vp8VideoCodec() : base("WebM (Vp8, Opus)", ".webm", Descr) { }

        public override FFmpegAudioArgsProvider AudioArgsProvider => FFmpegAudioItem.Opus;

        public override void Apply(FFmpegSettings Settings, VideoWriterArgs WriterArgs, FFmpegOutputArgs OutputArgs)
        {
            // quality: 63 (lowest) to 4 (highest)
            var crf = 63 - ((WriterArgs.VideoQuality - 1) * 59) / 99;

            OutputArgs.AddArg("vcodec", "libvpx")
                .AddArg("crf", crf)
                .AddArg("b:v", "1M");
        }
    }
}