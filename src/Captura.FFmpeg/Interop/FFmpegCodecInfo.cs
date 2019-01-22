using FFmpeg.AutoGen;

namespace Captura.FFmpeg.Interop
{
    public abstract unsafe class FFmpegCodecInfo
    {
        protected FFmpegCodecInfo(AVCodecID Id)
        {
            Codec = ffmpeg.avcodec_find_encoder(Id);
        }

        protected FFmpegCodecInfo(string Name)
        {
            Codec = ffmpeg.avcodec_find_encoder_by_name(Name);
        }

        public AVCodec* Codec { get; }
    }
}