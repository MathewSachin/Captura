using System.Linq;
using Captura.Models;

namespace Captura.FFmpeg
{
    class CustomFFmpegVideoCodec : FFmpegVideoCodec
    {
        readonly CustomFFmpegCodec _customCodec;

        public CustomFFmpegVideoCodec(CustomFFmpegCodec CustomCodec)
            : base("", "", "Custom Codec")
        {
            _customCodec = CustomCodec;
        }

        public override string Name => _customCodec.Name;

        public override string Extension => _customCodec.Extension;

        public override FFmpegAudioArgsProvider AudioArgsProvider => FFmpegAudioItem.Items
                                                                         .FirstOrDefault(M => M.Name == _customCodec.AudioFormat)
                                                                         ?.AudioArgsProvider ?? FFmpegAudioItem.Mp3;

        public override void Apply(FFmpegSettings Settings, VideoWriterArgs WriterArgs, FFmpegOutputArgs OutputArgs)
        {
            OutputArgs.AddArg(_customCodec.Args);
        }
    }
}