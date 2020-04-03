using Captura.Video;

namespace Captura.FFmpeg
{
    class Vp9VideoCodec : FFmpegVideoCodec
    {
        const string Descr = "Encode to WebM: Vp9 with Opus audio";

        public Vp9VideoCodec() : base("WebM (Vp9, Opus)", ".webm", Descr) { }

        public override FFmpegAudioArgsProvider AudioArgsProvider => FFmpegAudioItem.Opus;

        public override void Apply(FFmpegSettings Settings, VideoWriterArgs WriterArgs, FFmpegOutputArgs OutputArgs)
        {
            // quality: 63 (lowest) to 0 (highest)
            var crf = (63 * (100 - WriterArgs.VideoQuality)) / 99;

            OutputArgs.AddArg("vcodec", "libvpx-vp9")
                .AddArg("crf", crf)
                .AddArg("b:v", 0);
        }
    }
}