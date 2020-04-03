using System;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using AlphaMode = SharpDX.Direct2D1.AlphaMode;

namespace Captura.Windows.DesktopDuplication
{
    public class ColorPointerShape : IPointerShape
    {
        Bitmap _bmp;

        public ColorPointerShape(IntPtr ShapeBuffer,
            OutputDuplicatePointerShapeInformation ShapeInfo,
            RenderTarget RenderTarget)
        {
            _bmp = new Bitmap(RenderTarget,
                new Size2(ShapeInfo.Width, ShapeInfo.Height),
                new DataPointer(ShapeBuffer, ShapeInfo.Height * ShapeInfo.Pitch),
                ShapeInfo.Pitch,
                new BitmapProperties(new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied)));
        }

        public void Update(Texture2D DesktopTexture, OutputDuplicatePointerPosition PointerPosition) { }

        public Bitmap GetBitmap() => _bmp;

        public void Dispose()
        {
            _bmp.Dispose();
            _bmp = null;
        }
    }
}