using FFmpeg.AutoGen;

namespace Captura.FFmpeg.Interop
{
    public unsafe class FFmpegAudioStream : FFmpegStream
    {
        readonly AVFormatContext* _formatContext;
        readonly FFmpegAudioResampler _audioResampler;

        readonly ResamplerParams _sourceParams = new ResamplerParams
        {
            ChannelLayout = ffmpeg.AV_CH_LAYOUT_STEREO,
            SampleRate = 44100,
            SampleFormat = AVSampleFormat.AV_SAMPLE_FMT_S16
        };

        const int SrcChannelCount = 2;
        const int SrcBitsPerSample = 16;

        public FFmpegAudioStream(AVFormatContext* FormatContext,
            FFmpegAudioCodecInfo CodecInfo) : base(FormatContext, CodecInfo)
        {
            _formatContext = FormatContext;

            CodecContext->sample_fmt = CodecInfo.SampleFormat;
            CodecContext->sample_rate = _sourceParams.SampleRate;
            CodecContext->channel_layout = ffmpeg.AV_CH_LAYOUT_STEREO;

            _audioResampler = new FFmpegAudioResampler(_sourceParams,
                new ResamplerParams
                {
                    ChannelLayout = _sourceParams.ChannelLayout,
                    SampleRate = _sourceParams.SampleRate,
                    SampleFormat = CodecInfo.SampleFormat
                });
        }

        public void Write(byte[] Buffer, int Length)
        {
            fixed (byte* buffer = Buffer)
            {
                var data = new byte_ptrArray8 { [0] = buffer };

                var sampleCount = Length / SrcChannelCount / SrcBitsPerSample;

                var frame = new AVFrame
                {
                    data = data,
                    //linesize = new int_array8 { [0] = sampleCount },
                    format = (int) _sourceParams.SampleFormat,
                    nb_samples = sampleCount
                };

                var convertedFrame = _audioResampler.Convert(frame);

                Encode(convertedFrame);
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