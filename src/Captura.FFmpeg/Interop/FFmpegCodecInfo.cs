using FFmpeg.AutoGen;

namespace Captura.FFmpeg.Interop
{
    public class FFmpegCodecInfo
    {
        public FFmpegCodecInfo(AVCodecID Id)
        {
            this.Id = Id;
        }

        public AVCodecID Id { get; }
    }
}