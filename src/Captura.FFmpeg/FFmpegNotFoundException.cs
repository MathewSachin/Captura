using System;

namespace Captura.FFmpeg
{
    public class FFmpegNotFoundException : Exception
    {
        public override string Message => "FFmpeg could not be found. Please download using the in-built downloader.";
    }
}