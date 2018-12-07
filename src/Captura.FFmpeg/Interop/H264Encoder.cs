using System;
using System.Drawing;
using System.IO;
using Captura;

namespace FFmpeg.AutoGen.Example
{
    public sealed unsafe class H264VideoStreamEncoder : IDisposable, IVideoFileWriter
    {
        readonly int _fps;
        readonly Size _frameSize;
        readonly int _linesizeU;
        readonly int _linesizeV;
        readonly int _linesizeY;
        readonly AVCodec* _pCodec;
        readonly AVCodecContext* _pCodecContext;
        readonly Stream _stream;
        readonly int _uSize;
        readonly int _ySize;

        readonly VideoFrameConverter _vfc;

        int _frameNumber;

        void Write(byte[] BitmapData)
        {
            fixed (byte* pBitmapData = BitmapData)
            {
                var data = new byte_ptrArray8 { [0] = pBitmapData };
                var linesize = new int_array8 { [0] = BitmapData.Length / _frameSize.Height };

                var frame = new AVFrame
                {
                    data = data,
                    linesize = linesize,
                    format = (int) AVPixelFormat.AV_PIX_FMT_YUV420P,
                    height = _frameSize.Height
                };

                var convertedFrame = _vfc.Convert(frame);
                convertedFrame.pts = _frameNumber++ * _fps;
                Encode(convertedFrame);
            }
        }

        public H264VideoStreamEncoder(string FileName, int Fps, Size FrameSize)
        {
            _stream = File.Open(FileName, FileMode.Create);
            _fps = Fps;
            _frameSize = FrameSize;

            _pCodec = ffmpeg.avcodec_find_encoder_by_name("libx264");

            if (_pCodec == null)
                throw new InvalidOperationException("Codec not found.");

            _pCodecContext = ffmpeg.avcodec_alloc_context3(_pCodec);
            _pCodecContext->width = FrameSize.Width;
            _pCodecContext->height = FrameSize.Height;
            _pCodecContext->time_base = new AVRational { num = 1, den = Fps };
            _pCodecContext->pix_fmt = AVPixelFormat.AV_PIX_FMT_YUV420P;
            _pCodecContext->bit_rate = 4_00_000;
            _pCodecContext->gop_size = 10;
            _pCodecContext->max_b_frames = 1;
            ffmpeg.av_opt_set(_pCodecContext->priv_data, "preset", "ultrafast", 0);

            ffmpeg.avcodec_open2(_pCodecContext, _pCodec, null).ThrowExceptionIfError();

            _linesizeY = FrameSize.Width;
            _linesizeU = FrameSize.Width / 2;
            _linesizeV = FrameSize.Width / 2;

            _ySize = _linesizeY * FrameSize.Height;
            _uSize = _linesizeU * FrameSize.Height / 2;

            const AVPixelFormat sourcePixelFormat = AVPixelFormat.AV_PIX_FMT_BGRA;
            const AVPixelFormat destinationPixelFormat = AVPixelFormat.AV_PIX_FMT_YUV420P;

            _vfc = new VideoFrameConverter(FrameSize, sourcePixelFormat, FrameSize, destinationPixelFormat);
        }

        public void Dispose()
        {
            byte[] endcode = { 0, 0, 1, 0xb7 };

            _stream.Write(endcode, 0, endcode.Length);

            _stream.Dispose();

            _vfc.Dispose();

            ffmpeg.avcodec_close(_pCodecContext);
            ffmpeg.av_free(_pCodecContext);
            ffmpeg.av_free(_pCodec);
        }

        void CheckFrame(AVFrame Frame)
        {
            if (Frame.format != (int)_pCodecContext->pix_fmt)
                throw new ArgumentException("Invalid pixel format.", nameof(Frame));

            if (Frame.width != _frameSize.Width)
                throw new ArgumentException("Invalid width.", nameof(Frame));

            if (Frame.height != _frameSize.Height)
                throw new ArgumentException("Invalid height.", nameof(Frame));

            if (Frame.linesize[0] != _linesizeY)
                throw new ArgumentException("Invalid Y linesize.", nameof(Frame));

            if (Frame.linesize[1] != _linesizeU)
                throw new ArgumentException("Invalid U linesize.", nameof(Frame));

            if (Frame.linesize[2] != _linesizeV)
                throw new ArgumentException("Invalid V linesize.", nameof(Frame));

            if (Frame.data[1] - Frame.data[0] != _ySize)
                throw new ArgumentException("Invalid Y data size.", nameof(Frame));

            if (Frame.data[2] - Frame.data[1] != _uSize)
                throw new ArgumentException("Invalid U data size.", nameof(Frame));
        }

        void Encode(AVFrame Frame)
        {
            CheckFrame(Frame);

            var pPacket = ffmpeg.av_packet_alloc();
            try
            {
                int error;
                do
                {
                    ffmpeg.avcodec_send_frame(_pCodecContext, &Frame).ThrowExceptionIfError();

                    error = ffmpeg.avcodec_receive_packet(_pCodecContext, pPacket);
                } while (error == ffmpeg.AVERROR(ffmpeg.EAGAIN));

                error.ThrowExceptionIfError();

                using (var packetStream = new UnmanagedMemoryStream(pPacket->data, pPacket->size))
                    packetStream.CopyTo(_stream);
            }
            finally
            {
                ffmpeg.av_packet_free(&pPacket);
            }
        }

        byte[] _buffer;

        public void WriteFrame(IBitmapFrame Image)
        {
            if (Image is RepeatFrame)
            {
                ++_frameNumber;
                return;
            }

            if (_buffer == null)
            {
                _buffer = new byte[_frameSize.Width * _frameSize.Height * 4];
            }

            Write(_buffer);
        }

        public bool SupportsAudio { get; }

        public void WriteAudio(byte[] Buffer, int Length)
        {
        }
    }
}