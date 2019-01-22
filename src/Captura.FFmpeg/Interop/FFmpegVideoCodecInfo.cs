using FFmpeg.AutoGen;

namespace Captura.FFmpeg.Interop
{
    public class FFmpegVideoCodecInfo : FFmpegCodecInfo
    {
        public FFmpegVideoCodecInfo(AVCodecID Id, AVPixelFormat PixelFormat) : base(Id)
        {
            this.PixelFormat = PixelFormat;
        }

        public FFmpegVideoCodecInfo(string Name, AVPixelFormat PixelFormat) : base(Name)
        {
            this.PixelFormat = PixelFormat;
        }

        public AVPixelFormat PixelFormat { get; }
    }
}