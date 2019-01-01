using System;
using System.Drawing;
using System.Runtime.InteropServices;
using FFmpeg.AutoGen;

namespace Captura.FFmpeg.Interop
{
    public unsafe class FFmux : IVideoFileWriter
    {
        readonly FFmpegFormatContext _formatContext;

        readonly FFmpegVideoStream _videoStream;
        readonly FFmpegAudioStream _audioStream;

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        static readonly av_log_set_callback_callback LogCallback;

        static FFmux()
        {
            ffmpeg.av_log_set_level(ffmpeg.AV_LOG_VERBOSE);

            LogCallback = (p0, Level, Format, vl) =>
            {
                if (Level > ffmpeg.av_log_get_level())
                    return;

                const int lineSize = 1024;

                var lineBuffer = stackalloc byte[lineSize];
                var printPrefix = 1;

                ffmpeg.av_log_format_line(p0, Level, Format, vl, lineBuffer, lineSize, &printPrefix);

                var line = Marshal.PtrToStringAnsi((IntPtr)lineBuffer);

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(line);
                Console.ResetColor();
            };

            ffmpeg.av_log_set_callback(LogCallback);
        }

        public FFmux(string FileName, Size FrameSize, int Fps)
        {
            _formatContext = new FFmpegFormatContext(FileName, null);

            var fmt = _formatContext.FormatContext->oformat;

            if (fmt->video_codec != AVCodecID.AV_CODEC_ID_NONE)
            {
                var codecInfo = new FFmpegVideoCodecInfo(fmt->video_codec, AVPixelFormat.AV_PIX_FMT_YUV420P);

                _videoStream = new FFmpegVideoStream(_formatContext.FormatContext, codecInfo, Fps, FrameSize);

                SetVideoCodecOptions(_videoStream.CodecContext, codecInfo.Id);

                _videoStream.OpenCodec();
            }

            if (fmt->audio_codec != AVCodecID.AV_CODEC_ID_NONE)
            {
                var codecInfo = new FFmpegAudioCodecInfo(fmt->audio_codec, AVSampleFormat.AV_SAMPLE_FMT_FLTP);

                _audioStream = new FFmpegAudioStream(_formatContext.FormatContext, codecInfo);

                SetAudioCodecOptions(_audioStream.CodecContext, codecInfo.Id);

                _audioStream.OpenCodec();

                SupportsAudio = true;
            }

            _formatContext.BeginWriting();
        }

        void SetVideoCodecOptions(AVCodecContext* CodecContext, AVCodecID CodecId)
        {
            CodecContext->bit_rate = 4_000_000;
            CodecContext->gop_size = 12;
            CodecContext->max_b_frames = 1;

            if (CodecId == AVCodecID.AV_CODEC_ID_H264)
            {
                ffmpeg.av_opt_set(CodecContext->priv_data, "preset", "ultrafast", 0);
            }
        }

        void SetAudioCodecOptions(AVCodecContext* CodecContext, AVCodecID CodecId)
        {
            CodecContext->bit_rate = 64_000;
            CodecContext->strict_std_compliance = -2;
        }

        public void Dispose()
        {
            _formatContext.WriteTrailer();

            _videoStream.Dispose();
            _audioStream?.Dispose();

            _formatContext.Dispose();
        }

        public void WriteFrame(IBitmapFrame Image)
        {
            if (Image is RepeatFrame)
            {
                _videoStream.IncrementPts();
                return;
            }

            using (Image)
                Image.CopyTo(_videoStream.Buffer, _videoStream.Buffer.Length);

            _videoStream.WriteFrame();
        }

        public bool SupportsAudio { get; }

        public void WriteAudio(byte[] Buffer, int Length)
        {
            _audioStream.Write(Buffer, Length);
        }
    }
}