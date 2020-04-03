using System;
using System.Runtime.InteropServices;
using Captura.Windows.DirectX;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Rectangle = System.Drawing.Rectangle;

namespace Captura.Windows.DesktopDuplication
{
    public class DxMousePointer : IDisposable
    {
        readonly Direct2DEditorSession _editorSession;

        IntPtr _ptrShapeBuffer;
        int _ptrShapeBufferSize;
        OutputDuplicatePointerShapeInformation _ptrShapeInfo;
        OutputDuplicatePointerPosition _pointerPosition;
        IPointerShape _pointerShape;

        const int PtrShapeMonochrome = 1;
        const int PtrShapeColor = 2;
        const int PtrShapeMaskedColor = 4;

        public DxMousePointer(Direct2DEditorSession EditorSession)
        {
            _editorSession = EditorSession;
        }

        public void Update(Texture2D DesktopTexture, OutputDuplicateFrameInformation FrameInfo, OutputDuplication DeskDupl)
        {
            // No update
            if (FrameInfo.LastMouseUpdateTime == 0)
            {
                _pointerShape?.Update(DesktopTexture, _pointerPosition);
                return;
            }

            _pointerPosition = FrameInfo.PointerPosition;

            if (FrameInfo.PointerShapeBufferSize != 0)
            {
                if (FrameInfo.PointerShapeBufferSize > _ptrShapeBufferSize)
                {
                    _ptrShapeBufferSize = FrameInfo.PointerShapeBufferSize;

                    _ptrShapeBuffer = _ptrShapeBuffer != IntPtr.Zero
                        ? Marshal.ReAllocCoTaskMem(_ptrShapeBuffer, _ptrShapeBufferSize)
                        : Marshal.AllocCoTaskMem(_ptrShapeBufferSize);
                }

                DeskDupl.GetFramePointerShape(_ptrShapeBufferSize,
                    _ptrShapeBuffer,
                    out _,
                    out _ptrShapeInfo);

                _pointerShape?.Dispose();

                switch (_ptrShapeInfo.Type)
                {
                    case PtrShapeMonochrome:
                        _pointerShape = new MonochromePointerShape(_ptrShapeBuffer,
                            _ptrShapeInfo,
                            _editorSession);
                        break;

                    case PtrShapeMaskedColor:
                        _pointerShape = new MaskedColorPointerShape(_ptrShapeBuffer,
                            _ptrShapeInfo,
                            _editorSession);
                        break;

                    case PtrShapeColor:
                        _pointerShape = new ColorPointerShape(_ptrShapeBuffer,
                            _ptrShapeInfo,
                            _editorSession.RenderTarget);
                        break;
                }
            }

            _pointerShape?.Update(DesktopTexture, _pointerPosition);
        }

        public void Draw(Direct2DEditor Editor, Point Location = default)
        {
            if (!_pointerPosition.Visible)
                return;

            var bmp = _pointerShape?.GetBitmap();

            if (bmp == null)
                return;

            var rect = new Rectangle(_pointerPosition.Position.X + Location.X,
                _pointerPosition.Position.Y + Location.Y,
                (int) bmp.Size.Width,
                (int) bmp.Size.Height);

            Editor.DrawImage(new Direct2DImage(bmp), rect);
        }

        public void Dispose()
        {
            if (_ptrShapeBuffer == IntPtr.Zero)
                return;

            _pointerShape?.Dispose();
            Marshal.FreeCoTaskMem(_ptrShapeBuffer);
        }
    }
}