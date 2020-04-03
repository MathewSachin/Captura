using System.Linq;
using Captura.Video;

namespace Captura.FFmpeg
{
    class CustomFFmpegVideoCodec : FFmpegVideoCodec
    {
        readonly FFmpegCodecSettings _codecSettings;

        public CustomFFmpegVideoCodec(FFmpegCodecSettings CodecSettings)
            : base("", "", "Custom Codec")
        {
            _codecSettings = CodecSettings;
        }

        public override string Name => _codecSettings.Name;

        public override string Extension => _codecSettings.Extension;

        public override FFmpegAudioArgsProvider AudioArgsProvider => FFmpegAudioItem.Items
                                                                         .FirstOrDefault(M => M.Name == _codecSettings.AudioFormat)
                                                                         ?.AudioArgsProvider ?? FFmpegAudioItem.Mp3;

        public override void Apply(FFmpegSettings Settings, VideoWriterArgs WriterArgs, FFmpegOutputArgs OutputArgs)
        {
            OutputArgs.AddArg(_codecSettings.Args);
        }
    }
}