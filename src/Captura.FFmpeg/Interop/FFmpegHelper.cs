using System;
using System.Runtime.InteropServices;

namespace FFmpeg.AutoGen.Example
{
    static class FFmpegHelper
    {
        static unsafe string av_strerror(int Error)
        {
            const int bufferSize = 1024;
            var buffer = stackalloc byte[bufferSize];
            ffmpeg.av_strerror(Error, buffer, (ulong)bufferSize);
            var message = Marshal.PtrToStringAnsi((IntPtr)buffer);
            return message;
        }

        public static int ThrowExceptionIfError(this int Error)
        {
            if (Error < 0)
                throw new ApplicationException(av_strerror(Error));

            return Error;
        }
    }
}