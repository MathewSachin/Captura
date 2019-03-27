using System;

namespace Captura.FFmpeg
{
    public class FFmpegException : Exception
    {
        public FFmpegException(int ExitCode, Exception InnerException = null)
            : base($"An Error Occurred with FFmpeg, Exit Code: {ExitCode}.\nSee FFmpeg Log for more info.", InnerException) { }
    }
}