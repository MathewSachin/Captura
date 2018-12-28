using System.Drawing;
using System.Runtime.InteropServices;
using FFmpeg.AutoGen;

namespace Captura.FFmpeg.Interop
{
    public unsafe class FFmux : IVideoFileWriter
    {
        readonly FFmpegFormatContext _formatContext;
        readonly Size _frameSize;
        readonly int _fps;

        readonly FFmpegStream _videoStream;

        readonly VideoFrameConverter _vfc;

        public FFmux(string FileName, Size FrameSize, int Fps)
        {
            _frameSize = FrameSize;
            _fps = Fps;

            _formatContext = new FFmpegFormatContext(FileName, null);

            var fmt = _formatContext.FormatContext->oformat;

            if (fmt->video_codec != AVCodecID.AV_CODEC_ID_NONE)
            {
                var codecInfo = new FFmpegCodecInfo(fmt->video_codec);

                _videoStream = new FFmpegStream(_formatContext.FormatContext, codecInfo);

                SetVideoCodecOptions(_videoStream.CodecContext, codecInfo.Id);

                _videoStream.OpenCodec();
            }

            if (fmt->audio_codec != AVCodecID.AV_CODEC_ID_NONE)
            {
                //audio_st = AddStream(oc, out var audio_codec, fmt->audio_codec);
            }

            _formatContext.BeginWriting();

            if (_videoStream != null)
            {
                const AVPixelFormat sourcePixelFormat = AVPixelFormat.AV_PIX_FMT_BGRA;
                const AVPixelFormat destinationPixelFormat = AVPixelFormat.AV_PIX_FMT_YUV420P;

                _vfc = new VideoFrameConverter(FrameSize, sourcePixelFormat, FrameSize, destinationPixelFormat);

                InitFrame();
            }
        }

        void SetVideoCodecOptions(AVCodecContext* c, AVCodecID codec_id)
        {
            c->codec_id = codec_id;
            c->bit_rate = 4_000_000;
            c->width = _frameSize.Width;
            c->height = _frameSize.Height;
            c->time_base.num = 1;
            c->time_base.den = _fps;
            c->gop_size = 12;
            c->pix_fmt = AVPixelFormat.AV_PIX_FMT_YUV420P;
            c->max_b_frames = 1;

            if (codec_id == AVCodecID.AV_CODEC_ID_H264)
            {
                ffmpeg.av_opt_set(c->priv_data, "preset", "ultrafast", 0);
            }
        }

        void SetAudioCodecOptions(AVCodecContext* c, AVCodecID codec_id)
        {
            c->sample_fmt = AVSampleFormat.AV_SAMPLE_FMT_FLTP;
            c->bit_rate = 64_000;
            c->sample_rate = 44_100;
            c->channels = 2;
        }

        public void Dispose()
        {
            _formatContext.WriteTrailer();

            _videoStream.Dispose();
            //ffmpeg.avcodec_close(audioCodecContext);

            _formatContext.Dispose();

            _vfc.Dispose();

            _gcPin.Free();
            _buffer = null;
        }

        AVFrame _frame;
        long _pts;

        void InitFrame()
        {
            var dataLength = _frameSize.Height * _frameSize.Width * 4;

            _buffer = new byte[dataLength];
            _gcPin = GCHandle.Alloc(_buffer, GCHandleType.Pinned);

            var data = new byte_ptrArray8 { [0] = (byte*) _gcPin.AddrOfPinnedObject() };
            var linesize = new int_array8 { [0] = dataLength / _frameSize.Height };

            _frame = new AVFrame
            {
                data = data,
                linesize = linesize,
                format = (int)AVPixelFormat.AV_PIX_FMT_YUV420P,
                height = _frameSize.Height
            };
        }

        void Write()
        {
            var convertedFrame = _vfc.Convert(_frame);
            convertedFrame.pts = _pts;

            Encode(convertedFrame, _videoStream);

            IncrementPts();
        }

        void IncrementPts()
        {
            _pts += ffmpeg.av_rescale_q(1, _videoStream.Stream->codec->time_base, _videoStream.Stream->time_base);
        }

        void Encode(AVFrame Frame, FFmpegStream Stream)
        {
            var pPacket = ffmpeg.av_packet_alloc();
            try
            {
                int error;
                do
                {
                    ffmpeg.avcodec_send_frame(Stream.CodecContext, &Frame).ThrowExceptionIfError();

                    error = ffmpeg.avcodec_receive_packet(Stream.CodecContext, pPacket);
                } while (error == ffmpeg.AVERROR(ffmpeg.EAGAIN));

                error.ThrowExceptionIfError();

                pPacket->stream_index = Stream.Stream->index;
                pPacket->pts = Frame.pts;

                ffmpeg.av_write_frame(_formatContext.FormatContext, pPacket);
            }
            finally
            {
                ffmpeg.av_packet_free(&pPacket);
            }
        }

        byte[] _buffer;
        GCHandle _gcPin;

        public void WriteFrame(IBitmapFrame Image)
        {
            if (Image is RepeatFrame)
            {
                IncrementPts();
                return;
            }

            using (Image)
                Image.CopyTo(_buffer, _buffer.Length);

            Write();
        }

        public bool SupportsAudio => false;

        public void WriteAudio(byte[] Buffer, int Length) { }
    }
}