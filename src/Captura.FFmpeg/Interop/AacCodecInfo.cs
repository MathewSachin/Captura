using FFmpeg.AutoGen;

namespace Captura.FFmpeg.Interop
{
    public class AacCodecInfo : FFmpegAudioCodecInfo
    {
        public AacCodecInfo()
            : base(AVCodecID.AV_CODEC_ID_AAC, AVSampleFormat.AV_SAMPLE_FMT_FLTP)
        {
        }

        public override unsafe void SetOptions(FFmpegAudioStream AudioStream, int Quality)
        {
            var codecContext = AudioStream.CodecContext;

            codecContext->bit_rate = 64_000;
            codecContext->strict_std_compliance = -2;
        }
    }
}