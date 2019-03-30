using System.Drawing;
using System.Runtime.InteropServices;
using FFmpeg.AutoGen;

namespace Captura.FFmpeg.Interop
{
    public unsafe class FFmpegVideoStream : FFmpegStream
    {
        readonly AVFormatContext* _formatContext;
        readonly Size _frameSize;
        readonly FFmpegVideoScaler _vfc;

        AVFrame _frame;
        long _pts;
        GCHandle _gcPin;

        public byte[] Buffer { get; private set; }

        public FFmpegVideoStream(AVFormatContext* FormatContext,
            FFmpegVideoCodecInfo CodecInfo,
            int Fps,
            Size FrameSize,
            int Quality) : base(FormatContext, CodecInfo)
        {
            _formatContext = FormatContext;
            _frameSize = FrameSize;

            _vfc = new FFmpegVideoScaler(
                new ScalerParams
                {
                    Size = FrameSize,
                    PixelFormat = AVPixelFormat.AV_PIX_FMT_BGRA
                },
                new ScalerParams
                {
                    Size = FrameSize,
                    PixelFormat = CodecInfo.PixelFormat
                });

            InitFrame();

            CodecContext->codec_id = CodecInfo.Codec->id;
            CodecContext->width = FrameSize.Width;
            CodecContext->height = FrameSize.Height;
            CodecContext->pix_fmt = CodecInfo.PixelFormat;
            CodecContext->time_base.num = 1;
            CodecContext->time_base.den = Fps;

            CodecInfo.SetOptions(this, Quality);

            OpenCodec();
        }

        void InitFrame()
        {
            var dataLength = _frameSize.Height * _frameSize.Width * 4;

            Buffer = new byte[dataLength];
            _gcPin = GCHandle.Alloc(Buffer, GCHandleType.Pinned);

            var data = new byte_ptrArray8 { [0] = (byte*)_gcPin.AddrOfPinnedObject() };
            var linesize = new int_array8 { [0] = dataLength / _frameSize.Height };

            _frame = new AVFrame
            {
                data = data,
                linesize = linesize,
                height = _frameSize.Height
            };
        }

        public void WriteFrame()
        {
            var convertedFrame = _vfc.Convert(_frame);
            convertedFrame.pts = _pts;

            Encode(convertedFrame);

            IncrementPts();
        }

        public void IncrementPts()
        {
            _pts += ffmpeg.av_rescale_q(1, Stream->codec->time_base, Stream->time_base);
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
                pPacket->pts = Frame.pts;

                ffmpeg.av_write_frame(_formatContext, pPacket);
            }
            finally
            {
                ffmpeg.av_packet_free(&pPacket);
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            _vfc.Dispose();

            _gcPin.Free();
            Buffer = null;
        }
    }
}