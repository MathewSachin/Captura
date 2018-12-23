using System.IO;
using System.Runtime.InteropServices;
using Captura;
using SharpDX.Direct3D11;

namespace DesktopDuplication
{
    public class Texture2DFrame : IBitmapFrame
    {
        readonly Texture2D _texture;
        readonly Device _device;

        public Texture2DFrame(Texture2D Texture, Device Device)
        {
            _texture = Texture;
            _device = Device;

            Width = Texture.Description.Width;
            Height = Texture.Description.Height;
        }

        public void Dispose() { }

        public void SaveGif(Stream Stream)
        {
            throw new System.NotImplementedException();
        }

        public int Width { get; }
        public int Height { get; }

        public void CopyTo(byte[] Buffer, int Length)
        {
            var mapSource = _device.ImmediateContext.MapSubresource(_texture, 0, MapMode.Read, MapFlags.None);

            try
            {
                Marshal.Copy(mapSource.DataPointer, Buffer, 0, Length);
            }
            finally
            {
                _device.ImmediateContext.UnmapSubresource(_texture, 0);
            }
        }
    }
}