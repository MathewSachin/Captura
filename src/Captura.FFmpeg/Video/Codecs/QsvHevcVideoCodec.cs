using Captura.Video;

namespace Captura.FFmpeg
{
    class QsvHevcVideoCodec : FFmpegVideoCodec
    {
        const string Descr = "Encode to Mp4: HEVC (H.265) with AAC audio using Intel QuickSync hardware encoding.\nRequires the processor to be Skylake generation or later";

        public QsvHevcVideoCodec() : base("Intel QuickSync: Mp4 (HEVC, AAC)", ".mp4", Descr) { }

        public override FFmpegAudioArgsProvider AudioArgsProvider => FFmpegAudioItem.Aac;

        public override void Apply(FFmpegSettings Settings, VideoWriterArgs WriterArgs, FFmpegOutputArgs OutputArgs)
        {
            OutputArgs.AddArg("vcodec", "hevc_qsv")
                .AddArg("load_plugin", "hevc_hw")
                .AddArg("q", 2)
                .AddArg("preset:v", "veryfast");
        }
    }
}