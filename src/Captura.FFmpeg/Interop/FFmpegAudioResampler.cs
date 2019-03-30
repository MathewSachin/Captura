using System;
using FFmpeg.AutoGen;

namespace Captura.FFmpeg.Interop
{
    public unsafe class FFmpegAudioResampler : IDisposable
    {
        readonly ResamplerParams _sourceParams, _destParams;
        readonly SwrContext* _swrContext;
        byte** _buffer;
        int _bufferSampleSize;
        readonly int _destChannels;

        public FFmpegAudioResampler(ResamplerParams Source, ResamplerParams Destination)
        {
            _sourceParams = Source;
            _destParams = Destination;

            _destChannels = ffmpeg.av_get_channel_layout_nb_channels((ulong)Destination.ChannelLayout);

            _swrContext = ffmpeg.swr_alloc_set_opts(
                _swrContext,
                Destination.ChannelLayout,
                Destination.SampleFormat,
                Destination.SampleRate,
                Source.ChannelLayout,
                Source.SampleFormat,
                Source.SampleRate,
                0,
                null);

            ffmpeg.swr_init(_swrContext).ThrowExceptionIfError();
        }

        public void Convert(byte** Src, int SrcSampleCount, out byte** Dest, out int DestSampleCount)
        {
            var outSampleCount = (int) ffmpeg.av_rescale_rnd(
                ffmpeg.swr_get_delay(_swrContext, _sourceParams.SampleRate) + SrcSampleCount,
                _destParams.SampleRate,
                _sourceParams.SampleRate,
                AVRounding.AV_ROUND_UP);

            if (outSampleCount > _bufferSampleSize)
            {
                if (_buffer != null)
                    ffmpeg.av_freep(_buffer);

                var buffer = _buffer;

                ffmpeg.av_samples_alloc_array_and_samples(
                        &buffer,
                        null,
                        _destChannels,
                        outSampleCount,
                        _destParams.SampleFormat,
                        0)
                    .ThrowExceptionIfError();

                _buffer = buffer;
                _bufferSampleSize = outSampleCount;
            }

            ffmpeg.swr_convert(
                _swrContext,
                _buffer,
                outSampleCount,
                Src,
                SrcSampleCount);

            Dest = _buffer;
            DestSampleCount = outSampleCount;
        }

        public AVFrame Convert(AVFrame Frame)
        {
            fixed (byte** src = Frame.data.ToArray())
            {
                var srcSampleCount = Frame.nb_samples; //Frame.linesize[0];

                Convert(src, srcSampleCount, out var dest, out var destSampleCount);

                var destData = new byte_ptrArray8();

                for (var i = 0; i < _destChannels; ++i)
                    destData[(uint) i] = dest[i];

                return new AVFrame
                {
                    data = destData,
                    //linesize = new int_array8 { [0] = destSampleCount },
                    nb_samples = destSampleCount,
                    format = (int) _destParams.SampleFormat
                };
            }
        }

        public void Dispose()
        {
            if (_buffer != null)
            {
                var buffer = _buffer;

                ffmpeg.av_freep(&_buffer[0]);
                ffmpeg.av_freep(&buffer);

                _buffer = null;
            }

            var swrContext = _swrContext;
            ffmpeg.swr_free(&swrContext);
        }
    }
}