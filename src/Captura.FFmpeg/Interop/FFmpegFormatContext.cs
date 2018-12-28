using System;
using FFmpeg.AutoGen;

namespace Captura.FFmpeg.Interop
{
    public unsafe class FFmpegFormatContext : IDisposable
    {
        readonly string _fileName;

        public AVFormatContext* FormatContext { get; }

        public FFmpegFormatContext(string FileName, string Format)
        {
            _fileName = FileName;
            AVFormatContext* formatContext;

            ffmpeg.avformat_alloc_output_context2(&formatContext, null, Format, FileName);

            if (formatContext == null)
            {
                ffmpeg.avformat_alloc_output_context2(&formatContext, null, "mpeg", FileName);
            }

            if (formatContext == null)
            {
                throw new Exception("Failed to initialize Format Context");
            }

            FormatContext = formatContext;
        }

        public void BeginWriting()
        {
            ffmpeg.av_dump_format(FormatContext, 0, _fileName, 1);

            // Open the output file, if needed
            if ((FormatContext->oformat->flags & ffmpeg.AVFMT_NOFILE) == 0)
            {
                ffmpeg.avio_open(&FormatContext->pb, _fileName, ffmpeg.AVIO_FLAG_WRITE).ThrowExceptionIfError();
            }

            ffmpeg.avformat_write_header(FormatContext, null).ThrowExceptionIfError();
        }

        public void WriteTrailer()
        {
            ffmpeg.av_write_trailer(FormatContext);
        }

        public void Dispose()
        {
            if ((FormatContext->oformat->flags & ffmpeg.AVFMT_NOFILE) == 0)
            {
                ffmpeg.avio_close(FormatContext->pb);
            }

            ffmpeg.avformat_free_context(FormatContext);
        }
    }
}