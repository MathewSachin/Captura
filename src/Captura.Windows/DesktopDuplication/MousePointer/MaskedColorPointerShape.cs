using System;
using System.Runtime.InteropServices;
using Captura.Windows.DirectX;
using SharpDX.DXGI;

namespace Captura.Windows.DesktopDuplication
{
    public class MaskedColorPointerShape : MaskedPointerShape
    {
        byte[] _maskedShapeBuffer;

        public MaskedColorPointerShape(IntPtr ShapeBuffer,
            OutputDuplicatePointerShapeInformation ShapeInfo,
            Direct2DEditorSession EditorSession)
            : base(ShapeInfo.Width, ShapeInfo.Height, EditorSession)
        {
            _maskedShapeBuffer = new byte[Width * Height * 4];
            Marshal.Copy(ShapeBuffer, _maskedShapeBuffer, 0, _maskedShapeBuffer.Length);
        }

        protected override void OnUpdate()
        {
            for (var row = 0; row < Height; ++row)
            {
                for (var col = 0; col < Width; ++col)
                {
                    var index = row * Width * 4 + col * 4;

                    var mask = _maskedShapeBuffer[index + 3]; // Alpha value is mask

                    if (mask != 0) // 0xFF
                    {
                        // XOR with screen pixels
                        for (var i = 0; i < 3; ++i)
                        {
                            ShapeBuffer[index + i] = (byte)(DesktopBuffer[index + i] ^ _maskedShapeBuffer[index + i]);
                        }
                    }
                    else
                    {
                        // Replace screen pixels
                        for (var i = 0; i < 3; ++i)
                        {
                            ShapeBuffer[index + i] = _maskedShapeBuffer[index + i];
                        }
                    }

                    // Alpha
                    ShapeBuffer[index + 3] = 0xFF;
                }
            }
        }

        protected override void OnDispose()
        {
            _maskedShapeBuffer = null;
        }
    }
}