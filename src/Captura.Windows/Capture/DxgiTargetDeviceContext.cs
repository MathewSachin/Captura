using System;
using Captura.Windows.DirectX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace Captura.Video
{
    class DxgiTargetDeviceContext : ITargetDeviceContext
    {
        readonly Direct2DEditorSession _editorSession;
        readonly Texture2D _gdiCompatibleTexture;
        readonly Surface1 _dxgiSurface;

        public DxgiTargetDeviceContext(IPreviewWindow PreviewWindow, int Width, int Height)
        {
            _editorSession = new Direct2DEditorSession(Width, Height, PreviewWindow);
            _gdiCompatibleTexture = _editorSession.CreateGdiTexture(Width, Height);

            _dxgiSurface = _gdiCompatibleTexture.QueryInterface<Surface1>();
        }

        public IBitmapFrame DummyFrame => Texture2DFrame.DummyFrame;

        public void Dispose()
        {
            _dxgiSurface.Dispose();
            _gdiCompatibleTexture.Dispose();
            _editorSession.Dispose();
        }

        public IntPtr GetDC()
        {
            return _dxgiSurface.GetDC(true);
        }

        public IEditableFrame GetEditableFrame()
        {
            _dxgiSurface.ReleaseDC();

            _editorSession.Device.ImmediateContext.CopyResource(_gdiCompatibleTexture, _editorSession.DesktopTexture);
            _editorSession.Device.ImmediateContext.Flush();

            return new Direct2DEditor(_editorSession);
        }
    }
}
