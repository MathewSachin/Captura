using System.Drawing;
using Captura.FFmpeg.Interop;
using FFmpeg.AutoGen.Example;

namespace Captura.Models
{
    public class FFmpegInteropItem : IVideoWriterItem
    {
        public string Description => "Interop H.264";

        public string Extension => ".mp4";

        readonly string _name = "Interop H.264";
        public override string ToString() => _name;

        public virtual IVideoFileWriter GetVideoFileWriter(VideoWriterArgs Args)
        {
            var size = new Size(Args.ImageProvider.Width, Args.ImageProvider.Height);

            return new FFmux(Args.FileName, size, Args.FrameRate);
        }
    }
}
