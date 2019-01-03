using System;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using Bitmap = SharpDX.Direct2D1.Bitmap;
using MapFlags = SharpDX.Direct3D11.MapFlags;
using PixelFormat = SharpDX.Direct2D1.PixelFormat;
using Rectangle = System.Drawing.Rectangle;

namespace DesktopDuplication
{
    public class DxMousePointer : IDisposable
    {
        readonly Direct2DEditorSession _editorSession;

        IntPtr _ptrShapeBuffer;
        int _ptrShapeBufferSize;
        OutputDuplicatePointerShapeInformation _ptrShapeInfo;
        Bitmap _bmp;
        OutputDuplicatePointerPosition _pointerPosition;

        const int PtrShapeMonochrome = 1;
        const int PtrShapeColor = 2;
        const int PtrShapeMaskedColor = 4;

        public DxMousePointer(Direct2DEditorSession EditorSession)
        {
            _editorSession = EditorSession;
        }

        public void Update(OutputDuplicateFrameInformation FrameInfo, OutputDuplication DeskDupl)
        {
            // No update
            if (FrameInfo.LastMouseUpdateTime == 0)
                return;

            _pointerPosition = FrameInfo.PointerPosition;

            if (FrameInfo.PointerShapeBufferSize == 0)
                return;

            if (FrameInfo.PointerShapeBufferSize > _ptrShapeBufferSize)
            {
                _ptrShapeBufferSize = FrameInfo.PointerShapeBufferSize;

                _ptrShapeBuffer = _ptrShapeBuffer != IntPtr.Zero
                    ? Marshal.ReAllocCoTaskMem(_ptrShapeBuffer, _ptrShapeBufferSize)
                    : Marshal.AllocCoTaskMem(_ptrShapeBufferSize);
            }

            _bmp?.Dispose();

            DeskDupl.GetFramePointerShape(_ptrShapeBufferSize,
                _ptrShapeBuffer,
                out _,
                out _ptrShapeInfo);

            switch (_ptrShapeInfo.Type)
            {
                case PtrShapeMonochrome:
                    ProcessMask(true);
                    break;

                case PtrShapeMaskedColor:
                case PtrShapeColor:
                    _bmp = new Bitmap(_editorSession.RenderTarget,
                        new Size2(_ptrShapeInfo.Width, _ptrShapeInfo.Height),
                        new DataPointer(_ptrShapeBuffer, _ptrShapeInfo.Height * _ptrShapeInfo.Pitch),
                        _ptrShapeInfo.Pitch,
                        new BitmapProperties(new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied)));
                    break;
            }
        }

        void ProcessMask(bool Mono)
        {
            var width = _ptrShapeInfo.Width;
            var height = _ptrShapeInfo.Height / 2;

            var copyTexDesc = new Texture2DDescription
            {
                Width = width,
                Height = height,
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

            var shapeBuffer = new byte[width * height * 4];
            var desktopBuffer = new byte[width * height * 4];

            using (var copyTex = new Texture2D(_editorSession.Device, copyTexDesc))
            {
                var region = new ResourceRegion(_pointerPosition.Position.X,
                    _pointerPosition.Position.Y,
                    0,
                    _pointerPosition.Position.X + width,
                    _pointerPosition.Position.Y + height,
                    0);

                _editorSession.Device.ImmediateContext.CopySubresourceRegion(
                    _editorSession.StagingTexture,
                    0,
                    region,
                    copyTex,
                    0);

                var desktopMap = _editorSession.Device.ImmediateContext.MapSubresource(
                    copyTex,
                    0,
                    MapMode.Read,
                    MapFlags.None);

                try
                {
                    Marshal.Copy(desktopMap.DataPointer, desktopBuffer, 0, desktopBuffer.Length);
                }
                finally
                {
                    _editorSession.Device.ImmediateContext.UnmapSubresource(copyTex, 0);
                }
            }

            if (Mono)
            {
                var andMaskBuffer = new byte[width * height / 8];
                Marshal.Copy(_ptrShapeBuffer, andMaskBuffer, 0, andMaskBuffer.Length);

                var xorMaskBuffer = new byte[width * height / 8];
                Marshal.Copy(_ptrShapeBuffer + andMaskBuffer.Length, xorMaskBuffer, 0, xorMaskBuffer.Length);

                for (var j = 0; j < height; ++j)
                {
                    byte bit = 0x80;

                    for (var i = 0; i < width; ++i)
                    {
                        var maskIndex = j * width / 8 + i / 8;

                        var andMask = andMaskBuffer[maskIndex] & bit;
                        var xorMask = xorMaskBuffer[maskIndex] & bit;

                        var andMask32 = andMask != 0 ? new byte[] {0xFF, 0xFF, 0xFF, 0xFF} : new byte[] {0xFF, 0, 0, 0};
                        var xorMask32 = xorMask != 0 ? new byte[] {0, 0xFF, 0xFF, 0xFF} : new byte[4];

                        var index = j * width * 4 + i * 4;

                        for (var k = 0; k < 4; ++k)
                        {
                            shapeBuffer[index + 3 - k] = (byte)((desktopBuffer[index + 3 - k] & andMask32[k]) ^ xorMask32[k]);
                        }

                        if (0x01 == bit)
                        {
                            bit = 0x80;
                        }
                        else bit = (byte)(bit >> 1);
                    } /* cols */
                } /* rows */
            }

            var gcPin = GCHandle.Alloc(shapeBuffer, GCHandleType.Pinned);

            try
            {
                var pitch = width * 4;

                _bmp = new Bitmap(_editorSession.RenderTarget,
                    new Size2(width, height),
                    new DataPointer(gcPin.AddrOfPinnedObject(), height * pitch),
                    pitch,
                    new BitmapProperties(new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied)));
            }
            finally
            {
                gcPin.Free();
            }
        }

        public void Draw(Direct2DEditor Editor)
        {
            if (_bmp == null || !_pointerPosition.Visible)
                return;

            var rect = new Rectangle(_pointerPosition.Position.X,
                _pointerPosition.Position.Y,
                _ptrShapeInfo.Width,
                _ptrShapeInfo.Height);

            Editor.DrawImage(_bmp, rect);
        }

        public void Dispose()
        {
            if (_ptrShapeBuffer == IntPtr.Zero)
                return;

            _bmp.Dispose();
            Marshal.FreeCoTaskMem(_ptrShapeBuffer);
        }
    }
}