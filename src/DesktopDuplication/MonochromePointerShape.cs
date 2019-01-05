using System;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using MapFlags = SharpDX.Direct3D11.MapFlags;

namespace DesktopDuplication
{
    public class MonochromePointerShape : IPointerShape
    {
        Direct2DEditorSession _editorSession;
        readonly int _width, _height;
        Texture2D _copyTex;
        byte[] _shapeBuffer, _desktopBuffer, _andMaskBuffer, _xorMaskBuffer;

        Bitmap _bmp;

        public MonochromePointerShape(IntPtr ShapeBuffer,
            OutputDuplicatePointerShapeInformation ShapeInfo,
            Direct2DEditorSession EditorSession)
        {
            _editorSession = EditorSession;
            _width = ShapeInfo.Width;
            _height = ShapeInfo.Height / 2;

            var copyTexDesc = new Texture2DDescription
            {
                Width = _width,
                Height = _height,
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

            _shapeBuffer = new byte[_width * _height * 4];
            _desktopBuffer = new byte[_width * _height * 4];

            _copyTex = new Texture2D(_editorSession.Device, copyTexDesc);

            _andMaskBuffer = new byte[_width * _height / 8];
            Marshal.Copy(ShapeBuffer, _andMaskBuffer, 0, _andMaskBuffer.Length);

            _xorMaskBuffer = new byte[_width * _height / 8];
            Marshal.Copy(ShapeBuffer + _andMaskBuffer.Length, _xorMaskBuffer, 0, _xorMaskBuffer.Length);
        }

        // BGRA
        static readonly byte[] White = { 0xFF, 0xFF, 0xFF, 0xFF };
        static readonly byte[] Black = { 0, 0, 0, 0xFF };
        static readonly byte[] TransparentWhite = { 0xFF, 0xFF, 0xFF, 0 };
        static readonly byte[] TransparentBlack = new byte[4];

        public Bitmap GetBitmap() => _bmp;

        public void Update(Texture2D DesktopTexture, OutputDuplicatePointerPosition PointerPosition)
        {
            _bmp?.Dispose();

            var region = new ResourceRegion(
                PointerPosition.Position.X,
                PointerPosition.Position.Y,
                0,
                PointerPosition.Position.X + _width,
                PointerPosition.Position.Y + _height,
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
                Marshal.Copy(desktopMap.DataPointer, _desktopBuffer, 0, _desktopBuffer.Length);
            }
            finally
            {
                _editorSession.Device.ImmediateContext.UnmapSubresource(_copyTex, 0);
            }

            for (var row = 0; row < _height; ++row)
            {
                byte bit = 0x80;

                for (var col = 0; col < _width; ++col)
                {
                    var maskIndex = row * _width / 8 + col / 8;

                    var andMask = (_andMaskBuffer[maskIndex] & bit) == bit;
                    var xorMask = (_xorMaskBuffer[maskIndex] & bit) == bit;

                    var andMask32 = andMask ? White : Black;
                    var xorMask32 = xorMask ? TransparentWhite : TransparentBlack;

                    var index = row * _width * 4 + col * 4;

                    for (var k = 0; k < 4; ++k)
                    {
                        _shapeBuffer[index + k] = (byte)((_desktopBuffer[index + k] & andMask32[k]) ^ xorMask32[k]);
                    }

                    if (bit == 0x01)
                    {
                        bit = 0x80;
                    }
                    else bit = (byte)(bit >> 1);
                }
            }

            var gcPin = GCHandle.Alloc(_shapeBuffer, GCHandleType.Pinned);

            try
            {
                var pitch = _width * 4;

                _bmp = new Bitmap(_editorSession.RenderTarget,
                    new Size2(_width, _height),
                    new DataPointer(gcPin.AddrOfPinnedObject(), _height * pitch),
                    pitch,
                    new BitmapProperties(new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied)));
            }
            finally
            {
                gcPin.Free();
            }
        }

        public void Dispose()
        {
            _bmp?.Dispose();
            _bmp = null;

            _copyTex.Dispose();
            _copyTex = null;

            _shapeBuffer = _desktopBuffer = null;
            _andMaskBuffer = _xorMaskBuffer = null;
            _editorSession = null;
        }
    }
}