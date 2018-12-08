using System;
using System.Drawing;
using System.Runtime.InteropServices;
using FFmpeg.AutoGen;
using FFmpeg.AutoGen.Example;

namespace Captura.FFmpeg.Interop
{
    public unsafe class FFmux : IVideoFileWriter
    {
        int frame_count;
        readonly AVFormatContext* oc;
        readonly Size _frameSize;
        readonly int _fps;

        AVCodecContext* videoCodecContext, audioCodecContext;
        AVCodec* videoCodec, audioCodec;

        readonly AVStream* video_st;

        readonly VideoFrameConverter _vfc;

        public FFmux(string FileName, Size FrameSize, int Fps)
        {
            this._frameSize = FrameSize;
            _fps = Fps;

            AVFormatContext* formatContext;

            ffmpeg.avformat_alloc_output_context2(&formatContext, null, null, FileName);

            if (formatContext == null)
            {
                ffmpeg.avformat_alloc_output_context2(&formatContext, null, "mpeg", FileName);
            }

            if (formatContext == null)
            {
                throw new Exception("");
            }

            oc = formatContext;

            var fmt = oc->oformat;

            AVStream* audio_st = null;

            if (fmt->video_codec != AVCodecID.AV_CODEC_ID_NONE)
            {
                video_st = AddStream(fmt->video_codec);
            }

            if (fmt->audio_codec != AVCodecID.AV_CODEC_ID_NONE)
            {
                //audio_st = AddStream(oc, out var audio_codec, fmt->audio_codec);
            }

            if (video_st != null)
            {
            }

            if (audio_st != null)
            {
            }

            ffmpeg.av_dump_format(oc, 0, FileName, 1);
            
            // Open the output file, if needed
            if ((fmt->flags & ffmpeg.AVFMT_NOFILE) == 0)
            {
                ffmpeg.avio_open(&oc->pb, FileName, ffmpeg.AVIO_FLAG_WRITE).ThrowExceptionIfError();
            }

            ffmpeg.avformat_write_header(oc, null).ThrowExceptionIfError();

            const AVPixelFormat sourcePixelFormat = AVPixelFormat.AV_PIX_FMT_BGRA;
            const AVPixelFormat destinationPixelFormat = AVPixelFormat.AV_PIX_FMT_YUV420P;

            _vfc = new VideoFrameConverter(FrameSize, sourcePixelFormat, FrameSize, destinationPixelFormat);

            InitFrame();
        }

        AVStream* AddStream(AVCodecID codec_id)
        {
            var codec = ffmpeg.avcodec_find_encoder(codec_id);

            if (codec == null)
            {
                throw new Exception("Could not find codec");
            }

            var st = ffmpeg.avformat_new_stream(oc, codec);

            if (st == null)
            {
                throw new Exception("Could not allocate stream");
            }

            st->id = (int)(oc->nb_streams - 1);
            var c = st->codec;

            switch (codec->type)
            {
                case AVMediaType.AVMEDIA_TYPE_AUDIO:
                    c->sample_fmt = AVSampleFormat.AV_SAMPLE_FMT_FLTP;
                    c->bit_rate = 64_000;
                    c->sample_rate = 44_100;
                    c->channels = 2;

                    audioCodec = codec;
                    audioCodecContext = c;
                    break;

                case AVMediaType.AVMEDIA_TYPE_VIDEO:
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

                    ffmpeg.avcodec_open2(c, codec, null).ThrowExceptionIfError();

                    videoCodec = codec;
                    videoCodecContext = c;
                    break;
            }

            if ((oc->oformat->flags & ffmpeg.AVFMT_GLOBALHEADER) != 0)
            {
                c->flags = ffmpeg.AV_CODEC_FLAG_GLOBAL_HEADER;
            }

            return st;
        }

        public void Dispose()
        {
            ffmpeg.av_write_trailer(oc);

            ffmpeg.avcodec_close(videoCodecContext);

            //ffmpeg.avcodec_close(audioCodecContext);

            if ((oc->oformat->flags & ffmpeg.AVFMT_NOFILE) == 0)
            {
                ffmpeg.avio_close(oc->pb);
            }

            ffmpeg.avformat_free_context(oc);

            _vfc.Dispose();
        }

        AVFrame frame;

        void InitFrame()
        {
            _buffer = (byte*)Marshal.AllocHGlobal(_frameSize.Width * _frameSize.Height * 4);

            var dataLength = _frameSize.Height * _frameSize.Width * 4;

            var data = new byte_ptrArray8 { [0] = _buffer };
            var linesize = new int_array8 { [0] = dataLength / _frameSize.Height };

            frame = new AVFrame
            {
                data = data,
                linesize = linesize,
                format = (int)AVPixelFormat.AV_PIX_FMT_YUV420P,
                height = _frameSize.Height
            };
        }

        void Write()
        {
            var convertedFrame = _vfc.Convert(frame);
            convertedFrame.pts = frame_count++ * _fps * 100;

            Encode(convertedFrame);
        }

        void Encode(AVFrame Frame)
        {
            var pPacket = ffmpeg.av_packet_alloc();
            try
            {
                int error;
                do
                {
                    ffmpeg.avcodec_send_frame(videoCodecContext, &Frame).ThrowExceptionIfError();

                    error = ffmpeg.avcodec_receive_packet(videoCodecContext, pPacket);
                } while (error == ffmpeg.AVERROR(ffmpeg.EAGAIN));

                error.ThrowExceptionIfError();

                pPacket->stream_index = video_st->index;
                pPacket->pts = Frame.pts;

                ffmpeg.av_write_frame(oc, pPacket);
            }
            finally
            {
                ffmpeg.av_packet_free(&pPacket);
            }
        }

        byte* _buffer;

        public void WriteFrame(IBitmapFrame Image)
        {
            if (Image is RepeatFrame)
            {
                ++frame_count;
                return;
            }

            using (Image)
                Image.CopyTo((IntPtr) _buffer);

            Write();
        }

        public bool SupportsAudio { get; }

        public void WriteAudio(byte[] Buffer, int Length) { }
    }
}