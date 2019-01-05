using System;
using SharpDX.Direct2D1;
using SharpDX.DXGI;

namespace DesktopDuplication
{
    public class MaskedColorPointerShape : ColorPointerShape
    {
        public MaskedColorPointerShape(IntPtr ShapeBuffer, OutputDuplicatePointerShapeInformation ShapeInfo, RenderTarget RenderTarget)
            : base(ShapeBuffer, ShapeInfo, RenderTarget) { }
    }
}