using Captura.FFmpeg.Interop;

namespace Captura.Models
{
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class FFmpegInteropItem : IVideoWriterItem
    {
        public string Description => "Interop H.264";

        public string Extension => ".mp4";

        readonly string _name = "Interop H.264";
        public override string ToString() => _name;

        public virtual IVideoFileWriter GetVideoFileWriter(VideoWriterArgs Args)
        {
            // Worked!: var codecInfo = new FFmpegVideoCodecInfo("h264_qsv", AVPixelFormat.AV_PIX_FMT_NV12);
            var videoCodec = new X264CodecInfo();

            return new FFmux(Args, videoCodec);
        }
    }
}
