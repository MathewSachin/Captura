using System;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DXGI;
using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using Bitmap = SharpDX.Direct2D1.Bitmap;
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

            _bmp = new Bitmap(_editorSession.RenderTarget,
                new Size2(_ptrShapeInfo.Width, _ptrShapeInfo.Height),
                new DataPointer(_ptrShapeBuffer, _ptrShapeInfo.Height * _ptrShapeInfo.Pitch),
                _ptrShapeInfo.Pitch,
                new BitmapProperties(new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied)));
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