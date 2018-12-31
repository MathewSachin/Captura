using System;
using FFmpeg.AutoGen;

namespace Captura.FFmpeg.Interop
{
    public unsafe class FFmpegAudioResampler : IDisposable
    {
        readonly SwrContext* _swrContext;
        readonly byte** _srcData, _destData;
        int _srcLinesize, _destLinesize;
        readonly AVSampleFormat _destSampleFormat;

        public FFmpegAudioResampler(ResamplerParams Source, ResamplerParams Destination)
        {
            _destSampleFormat = Destination.SampleFormat;

            _swrContext = ffmpeg.swr_alloc();

            ffmpeg.av_opt_set_int(_swrContext, "in_channel_layout", Source.ChannelLayout, 0);
            ffmpeg.av_opt_set_int(_swrContext, "in_sample_rate", Source.SampleRate, 0);
            ffmpeg.av_opt_set_sample_fmt(_swrContext, "in_sample_fmt", Source.SampleFormat, 0);

            ffmpeg.av_opt_set_int(_swrContext, "out_channel_layout", Destination.ChannelLayout, 0);
            ffmpeg.av_opt_set_int(_swrContext, "out_sample_rate", Destination.SampleRate, 0);
            ffmpeg.av_opt_set_sample_fmt(_swrContext, "out_sample_fmt", Destination.SampleFormat, 0);

            ffmpeg.swr_init(_swrContext).ThrowExceptionIfError();

            var srcChannels = ffmpeg.av_get_channel_layout_nb_channels((ulong) Source.ChannelLayout);

            byte** data = null;
            var linesize = 0;

            var srcSampleCount = 1024;

            ffmpeg.av_samples_alloc_array_and_samples(
                    &data,
                    &linesize,
                    srcChannels,
                    srcSampleCount,
                    Source.SampleFormat,
                    0)
                .ThrowExceptionIfError();

            _srcData = data;
            _srcLinesize = linesize;

            long maxDestSampleCount, destSampleCount;

            maxDestSampleCount = destSampleCount = ffmpeg.av_rescale_rnd(srcSampleCount, Destination.SampleRate,
                Source.SampleRate, AVRounding.AV_ROUND_UP);

            var destChannels = ffmpeg.av_get_channel_layout_nb_channels((ulong) Destination.ChannelLayout);

            ffmpeg.av_samples_alloc_array_and_samples(
                    &data,
                    &linesize,
                    destChannels,
                    (int) destSampleCount,
                    Destination.SampleFormat,
                    0)
                .ThrowExceptionIfError();

            _destData = data;
            _destLinesize = linesize;
        }

        public AVFrame Convert(AVFrame Frame)
        {
            _srcData[0] = Frame.data[0];
            _srcLinesize = Frame.linesize[0];

            ffmpeg.swr_convert(_swrContext,
                _destData,
                _destLinesize,
                _srcData,
                _srcLinesize);

            return new AVFrame
            {
                data = new byte_ptrArray8 { [0] = _destData[0] },
                linesize = new int_array8 { [0] = _destLinesize },
                format = (int) _destSampleFormat
            };
        }

        public void Dispose()
        {
            var ptr = _srcData;

            if (ptr != null)
            {
                ffmpeg.av_free(&ptr[0]);
            }

            ffmpeg.av_free(ptr);

            ptr = _destData;

            if (ptr != null)
            {
                ffmpeg.av_free(&ptr[0]);
            }

            ffmpeg.av_free(ptr);

            var swrContext = _swrContext;
            ffmpeg.swr_free(&swrContext);
        }
    }
}