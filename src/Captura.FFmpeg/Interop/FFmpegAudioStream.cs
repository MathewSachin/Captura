using FFmpeg.AutoGen;

namespace Captura.FFmpeg.Interop
{
    public unsafe class FFmpegAudioStream : FFmpegStream
    {
        readonly AVFormatContext* _formatContext;

        const AVSampleFormat SampleFormat = AVSampleFormat.AV_SAMPLE_FMT_S16;

        public FFmpegAudioStream(AVFormatContext* FormatContext,
            FFmpegCodecInfo CodecInfo) : base(FormatContext, CodecInfo)
        {
            _formatContext = FormatContext;

            CodecContext->sample_fmt = SampleFormat;
            CodecContext->sample_rate = 44_100;
            CodecContext->channels = 2;

            // TODO: Write AudioResampler. MPEG AAC requires FLTP
        }

        public void Write(byte[] Buffer, int Length)
        {
            fixed (byte* buffer = Buffer)
            {
                var data = new byte_ptrArray8 { [0] = buffer };

                var frame = new AVFrame
                {
                    data = data,
                    linesize = new int_array8 { [0] = Length },
                    format = (int) SampleFormat
                };

                Encode(frame);
            }
        }

        void Encode(AVFrame Frame)
        {
            var pPacket = ffmpeg.av_packet_alloc();
            try
            {
                int error;
                do
                {
                    ffmpeg.avcodec_send_frame(CodecContext, &Frame).ThrowExceptionIfError();

                    error = ffmpeg.avcodec_receive_packet(CodecContext, pPacket);
                } while (error == ffmpeg.AVERROR(ffmpeg.EAGAIN));

                error.ThrowExceptionIfError();

                pPacket->stream_index = Stream->index;

                ffmpeg.av_write_frame(_formatContext, pPacket);
            }
            finally
            {
                ffmpeg.av_packet_free(&pPacket);
            }
        }
    }
}