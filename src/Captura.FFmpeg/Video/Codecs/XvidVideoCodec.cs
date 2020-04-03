using Captura.Video;

namespace Captura.FFmpeg
{
    class XvidVideoCodec : FFmpegVideoCodec
    {
        const string Descr = "Encode to Avi with Mp3 audio using Xvid encoder";

        public XvidVideoCodec() : base("Avi (Xvid, Mp3)", ".avi", Descr) { }

        public override FFmpegAudioArgsProvider AudioArgsProvider => FFmpegAudioItem.Mp3;

        public override void Apply(FFmpegSettings Settings, VideoWriterArgs WriterArgs, FFmpegOutputArgs OutputArgs)
        {
            // quality: 31 (lowest) to 1 (highest)
            var qscale = 31 - ((WriterArgs.VideoQuality - 1) * 30) / 99;

            OutputArgs.AddArg("vcodec", "libxvid")
                .AddArg("qscale:v", qscale);
        }
    }
}