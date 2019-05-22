using System;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace DesktopDuplication
{
    public interface IPointerShape : IDisposable
    {
        void Update(Texture2D DesktopTexture, OutputDuplicatePointerPosition PointerPosition);

        Bitmap GetBitmap();
    }
}