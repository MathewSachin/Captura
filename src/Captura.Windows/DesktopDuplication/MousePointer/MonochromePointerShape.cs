using System;
using System.Runtime.InteropServices;
using Captura.Windows.DirectX;
using SharpDX.DXGI;

namespace Captura.Windows.DesktopDuplication
{
    public class MonochromePointerShape : MaskedPointerShape
    {
        byte[] _andMaskBuffer, _xorMaskBuffer;

        public MonochromePointerShape(IntPtr ShapeBuffer,
            OutputDuplicatePointerShapeInformation ShapeInfo,
            Direct2DEditorSession EditorSession)
            : base(ShapeInfo.Width, ShapeInfo.Height / 2, EditorSession)
        {
            _andMaskBuffer = new byte[Width * Height / 8];
            Marshal.Copy(ShapeBuffer, _andMaskBuffer, 0, _andMaskBuffer.Length);

            _xorMaskBuffer = new byte[Width * Height / 8];
            Marshal.Copy(ShapeBuffer + _andMaskBuffer.Length, _xorMaskBuffer, 0, _xorMaskBuffer.Length);
        }

        // BGRA
        static readonly byte[] White = { 0xFF, 0xFF, 0xFF, 0xFF };
        static readonly byte[] Black = { 0, 0, 0, 0xFF };
        static readonly byte[] TransparentWhite = { 0xFF, 0xFF, 0xFF, 0 };
        static readonly byte[] TransparentBlack = new byte[4];

        protected override void OnUpdate()
        {
            for (var row = 0; row < Height; ++row)
            {
                byte bit = 0x80;

                for (var col = 0; col < Width; ++col)
                {
                    var maskIndex = row * Width / 8 + col / 8;

                    var andMask = (_andMaskBuffer[maskIndex] & bit) == bit;
                    var xorMask = (_xorMaskBuffer[maskIndex] & bit) == bit;

                    var andMask32 = andMask ? White : Black;
                    var xorMask32 = xorMask ? TransparentWhite : TransparentBlack;

                    var index = row * Width * 4 + col * 4;

                    for (var k = 0; k < 4; ++k)
                    {
                        ShapeBuffer[index + k] = (byte)((DesktopBuffer[index + k] & andMask32[k]) ^ xorMask32[k]);
                    }

                    if (bit == 0x01)
                    {
                        bit = 0x80;
                    }
                    else bit = (byte)(bit >> 1);
                }
            }
        }

        protected override void OnDispose()
        {
            _andMaskBuffer = _xorMaskBuffer = null;
        }
    }
}