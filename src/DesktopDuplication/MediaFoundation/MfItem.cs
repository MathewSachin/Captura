using DesktopDuplication;
using SharpDX.Direct3D11;

namespace Captura.Models
{
    public class MfItem : IVideoWriterItem
    {
        readonly Device _device;

        public string Extension => ".mp4";
        public string Description { get; } = "mp4";

        readonly string _name = "mf";

        public MfItem(Device Device)
        {
            _device = Device;
        }

        public override string ToString() => _name;

        public virtual IVideoFileWriter GetVideoFileWriter(VideoWriterArgs Args)
        {
            return new MfWriter(Args.FrameRate,
                Args.ImageProvider.Width,
                Args.ImageProvider.Height,
                Args.FileName,
                _device);
        }
    }
}