using System.Runtime.InteropServices;
using Captura.Windows.DirectX;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using MapFlags = SharpDX.Direct3D11.MapFlags;

namespace Captura.Windows.DesktopDuplication
{
    public abstract class MaskedPointerShape : IPointerShape
    {
        Direct2DEditorSession _editorSession;
        Texture2D _copyTex;
        Bitmap _bmp;
        protected byte[] ShapeBuffer, DesktopBuffer;

        protected int Width { get; }
        protected int Height { get; }

        public MaskedPointerShape(int Width, int Height, Direct2DEditorSession EditorSession)
        {
            this.Width = Width;
            this.Height = Height;
            _editorSession = EditorSession;

            var copyTexDesc = new Texture2DDescription
            {
                Width = this.Width,
                Height = this.Height,
                MipLevels = 1,
                ArraySize = 1,
                Format = Format.B8G8R8A8_UNorm,
                SampleDescription =
                {
                    Count = 1,
                    Quality = 0
                },
                Usage = ResourceUsage.Staging,
                BindFlags = 0,
                CpuAccessFlags = CpuAccessFlags.Read
            };

            _copyTex = new Texture2D(_editorSession.Device, copyTexDesc);

            ShapeBuffer = new byte[Width * Height * 4];
            DesktopBuffer = new byte[Width * Height * 4];
        }

        public Bitmap GetBitmap() => _bmp;

        public void Update(Texture2D DesktopTexture, OutputDuplicatePointerPosition PointerPosition)
        {
            _bmp?.Dispose();

            var region = new ResourceRegion(
                PointerPosition.Position.X,
                PointerPosition.Position.Y,
                0,
                PointerPosition.Position.X + Width,
                PointerPosition.Position.Y + Height,
                1);

            _editorSession.Device.ImmediateContext.CopySubresourceRegion(
                DesktopTexture,
                0,
                region,
                _copyTex,
                0);

            var desktopMap = _editorSession.Device.ImmediateContext.MapSubresource(
                _copyTex,
                0,
                MapMode.Read,
                MapFlags.None);

            try
            {
                Marshal.Copy(desktopMap.DataPointer, DesktopBuffer, 0, DesktopBuffer.Length);
            }
            finally
            {
                _editorSession.Device.ImmediateContext.UnmapSubresource(_copyTex, 0);
            }

            OnUpdate();

            var gcPin = GCHandle.Alloc(ShapeBuffer, GCHandleType.Pinned);

            try
            {
                var pitch = Width * 4;

                _bmp = new Bitmap(_editorSession.RenderTarget,
                    new Size2(Width, Height),
                    new DataPointer(gcPin.AddrOfPinnedObject(), Height * pitch),
                    pitch,
                    new BitmapProperties(new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied)));
            }
            finally
            {
                gcPin.Free();
            }
        }

        protected abstract void OnUpdate();
        protected abstract void OnDispose();

        public void Dispose()
        {
            _bmp?.Dispose();
            _bmp = null;

            _copyTex?.Dispose();
            _copyTex = null;

            ShapeBuffer = DesktopBuffer = null;

            _editorSession = null;

            OnDispose();
        }
    }
}