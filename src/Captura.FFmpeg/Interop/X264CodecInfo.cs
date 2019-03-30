using FFmpeg.AutoGen;

namespace Captura.FFmpeg.Interop
{
    public unsafe class X264CodecInfo : FFmpegVideoCodecInfo
    {
        public X264CodecInfo()
            : base("libx264", AVPixelFormat.AV_PIX_FMT_YUV420P)
        {
        }

        public override void SetOptions(FFmpegVideoStream VideoStream, int Quality)
        {
            var codecContext = VideoStream.CodecContext;

            codecContext->gop_size = 12;
            codecContext->max_b_frames = 1;

            ffmpeg.av_opt_set(codecContext->priv_data, "preset", "ultrafast", 0);

            var crf = (51 * (100 - Quality)) / 99;
            ffmpeg.av_opt_set(codecContext->priv_data, "crf", crf.ToString(), 0);
        }

        public override FFmpegAudioCodecInfo AudioCodec { get; } = new AacCodecInfo();
    }
}