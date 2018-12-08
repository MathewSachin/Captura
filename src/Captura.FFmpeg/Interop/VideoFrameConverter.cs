using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace FFmpeg.AutoGen.Example
{
    public sealed unsafe class VideoFrameConverter : IDisposable
    {
        readonly IntPtr _convertedFrameBufferPtr;
        readonly Size _destinationSize;
        readonly byte_ptrArray4 _dstData;
        readonly int_array4 _dstLinesize;
        readonly SwsContext* _pConvertContext;

        public VideoFrameConverter(Size SourceSize,
            AVPixelFormat SourcePixelFormat,
            Size DestinationSize,
            AVPixelFormat DestinationPixelFormat)
        {
            _destinationSize = DestinationSize;

            _pConvertContext = ffmpeg.sws_getContext(SourceSize.Width,
                SourceSize.Height,
                SourcePixelFormat,
                DestinationSize.Width,
                DestinationSize.Height,
                DestinationPixelFormat,
                ffmpeg.SWS_FAST_BILINEAR, null, null, null);

            if (_pConvertContext == null)
                throw new ApplicationException("Could not initialize the conversion context.");

            var convertedFrameBufferSize = ffmpeg.av_image_get_buffer_size(DestinationPixelFormat, DestinationSize.Width, DestinationSize.Height, 1);
            _convertedFrameBufferPtr = Marshal.AllocHGlobal(convertedFrameBufferSize);
            _dstData = new byte_ptrArray4();
            _dstLinesize = new int_array4();

            ffmpeg.av_image_fill_arrays(ref _dstData, ref _dstLinesize, (byte*)_convertedFrameBufferPtr, DestinationPixelFormat, DestinationSize.Width, DestinationSize.Height, 1);
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal(_convertedFrameBufferPtr);
            ffmpeg.sws_freeContext(_pConvertContext);
        }

        public AVFrame Convert(AVFrame SourceFrame)
        {
            ffmpeg.sws_scale(_pConvertContext, SourceFrame.data, SourceFrame.linesize, 0, SourceFrame.height, _dstData, _dstLinesize);

            var data = new byte_ptrArray8();
            data.UpdateFrom(_dstData);
            var linesize = new int_array8();
            linesize.UpdateFrom(_dstLinesize);

            return new AVFrame
            {
                data = data,
                linesize = linesize,
                width = _destinationSize.Width,
                height = _destinationSize.Height
            };
        }
    }
}
