using System;
using FFmpeg.AutoGen;
using FFmpeg.AutoGen.Example;

namespace Captura.FFmpeg.Interop
{
    public unsafe class FFmux : IDisposable
    {
        AVFrame* frame;
        AVFormatContext** oc;

        public FFmux(string FileName)
        {
            ffmpeg.avformat_alloc_output_context2(oc, null, null, FileName);

            if (oc == null)
            {
                ffmpeg.avformat_alloc_output_context2(oc, null, "mpeg", FileName);
            }

            if (oc == null)
            {
                throw new Exception("");
            }

            var fmt = (*oc)->oformat;

            AVStream* audio_st = null, video_st = null;

            if (fmt->video_codec != AVCodecID.AV_CODEC_ID_NONE)
            {
                video_st = AddStream(out var video_codec, fmt->video_codec);
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

            ffmpeg.av_dump_format(*oc, 0, FileName, 1);

            ffmpeg.avformat_write_header(*oc, null).ThrowExceptionIfError();

            if (frame != null)
            {
                frame->pts = 0;
            }
        }

        AVStream* AddStream(out AVCodec* codec, AVCodecID codec_id)
        {
            codec = ffmpeg.avcodec_find_encoder(codec_id);

            if (codec == null)
            {
                throw new Exception("Could not find codec");
            }

            var st = ffmpeg.avformat_new_stream(*oc, codec);

            if (st == null)
            {
                throw new Exception("Could not allocate stream");
            }

            st->id = (int)((*oc)->nb_streams - 1);
            var c = st->codec;

            switch (codec->type)
            {
                case AVMediaType.AVMEDIA_TYPE_AUDIO:
                    c->sample_fmt = AVSampleFormat.AV_SAMPLE_FMT_FLTP;
                    c->bit_rate = 64_000;
                    c->sample_rate = 44_100;
                    c->channels = 2;
                    break;

                case AVMediaType.AVMEDIA_TYPE_VIDEO:
                    c->codec_id = codec_id;
                    c->bit_rate = 4_00_000;
                    c->width = 1920;
                    c->height = 1080;
                    c->time_base.num = 1;
                    c->gop_size = 12;
                    c->pix_fmt = AVPixelFormat.AV_PIX_FMT_YUV420P;
                    break;
            }

            if (((*oc)->oformat->flags & ffmpeg.AVFMT_GLOBALHEADER) != 0)
            {
                c->flags = ffmpeg.AV_CODEC_FLAG_GLOBAL_HEADER;
            }

            return st;
        }

        public void Dispose()
        {
            ffmpeg.av_write_trailer(*oc);

            ffmpeg.avformat_free_context(*oc);
        }
    }
}