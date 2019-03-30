using FFmpeg.AutoGen;

namespace Captura.FFmpeg.Interop
{
    public abstract class FFmpegVideoCodecInfo : FFmpegCodecInfo
    {
        protected FFmpegVideoCodecInfo(AVCodecID Id, AVPixelFormat PixelFormat) : base(Id)
        {
            this.PixelFormat = PixelFormat;
        }

        protected FFmpegVideoCodecInfo(string Name, AVPixelFormat PixelFormat) : base(Name)
        {
            this.PixelFormat = PixelFormat;
        }

        public virtual string Format { get; }

        public virtual AVPixelFormat PixelFormat { get; }

        public virtual FFmpegAudioCodecInfo AudioCodec { get; }

        public abstract void SetOptions(FFmpegVideoStream VideoStream, int Quality);
    }
}