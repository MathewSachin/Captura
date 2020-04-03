using Captura.Video;

namespace Captura.FFmpeg
{
    class NvencVideoCodec : FFmpegVideoCodec
    {
        readonly string _fFmpegCodecName;

        const string NVencSupport = "If this doesn't work, please check on NVIDIA's website whether your graphic card supports NVenc.";

        NvencVideoCodec(string Name, string FFmpegCodecName, string Description)
            : base(Name, ".mp4", $"{Description}\n{NVencSupport}")
        {
            _fFmpegCodecName = FFmpegCodecName;
        }

        public override FFmpegAudioArgsProvider AudioArgsProvider => FFmpegAudioItem.Aac;

        public override void Apply(FFmpegSettings Settings, VideoWriterArgs WriterArgs, FFmpegOutputArgs OutputArgs)
        {
            OutputArgs.AddArg("c:v", _fFmpegCodecName)
                .AddArg("pixel_format", "yuv444p")
                .AddArg("preset", "fast");
        }

        public static NvencVideoCodec CreateH264()
        {
            return new NvencVideoCodec("NVenc: Mp4 (H.264, AAC)", "h264_nvenc", "Encode to Mp4: H.264 with AAC audio using NVenc");
        }

        public static NvencVideoCodec CreateHevc()
        {
            return new NvencVideoCodec("NVenc: Mp4 (HEVC, AAC)", "hevc_nvenc", "Encode to Mp4: HEVC with AAC audio using NVenc");
        }
    }
}