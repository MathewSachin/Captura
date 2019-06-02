using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Captura.Models;
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
            ffmpeg.av_log_set_level(ffmpeg.AV_LOG_DEBUG);

            LogCallback = (p0, Level, Format, vl) =>
            {
                if (Level > ffmpeg.av_log_get_level())
                    return;

                const int lineSize = 1024;

                var lineBuffer = stackalloc byte[lineSize];
                var printPrefix = 1;

                ffmpeg.av_log_format_line(p0, Level, Format, vl, lineBuffer, lineSize, &printPrefix);

                var line = Marshal.PtrToStringAnsi((IntPtr)lineBuffer);

                Console.Write(line);
            };

            ffmpeg.av_log_set_callback(LogCallback);
        }

        public FFmux(VideoWriterArgs Args, FFmpegVideoCodecInfo VideoCodec)
        {
            _formatContext = new FFmpegFormatContext(Args.FileName, VideoCodec.Format);

            var size = new Size(Args.ImageProvider.Width, Args.ImageProvider.Height);

            _videoStream = new FFmpegVideoStream(_formatContext.FormatContext,
                VideoCodec,
                Args.FrameRate,
                size,
                Args.VideoQuality);

            if (Args.AudioProvider != null && VideoCodec.AudioCodec != null)
            {
                _audioStream = new FFmpegAudioStream(_formatContext.FormatContext,
                    VideoCodec.AudioCodec,
                    Args.AudioQuality);

                SupportsAudio = true;
            }

            _formatContext.BeginWriting();
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
                Image.CopyTo(_videoStream.Buffer);

            _videoStream.WriteFrame();
        }

        public bool SupportsAudio { get; }

        public void WriteAudio(byte[] Buffer, int Length)
        {
            _audioStream.Write(Buffer, Length);
        }
    }
}