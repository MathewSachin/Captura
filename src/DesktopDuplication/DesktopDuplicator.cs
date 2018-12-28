// Adapted from https://github.com/jasonpang/desktop-duplication-net

using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Drawing;
using Captura;
using SharpDX.Direct3D;
using Device = SharpDX.Direct3D11.Device;

namespace DesktopDuplication
{
    public class DesktopDuplicator : IDisposable
    {
        readonly Texture2D _desktopImageTexture;
        readonly Texture2D _stagingTexture;
        readonly Direct2DEditorSession _editorSession;
        readonly DxMousePointer _mousePointer;
        readonly DuplCapture _duplCapture;
        readonly Device _device;
        readonly Device _deviceForDeskDupl;

        public int Timeout { get; set; }

        public DesktopDuplicator(Rectangle Rect, bool IncludeCursor, Output1 Output)
        {
            _device = new Device(DriverType.Hardware,
                DeviceCreationFlags.BgraSupport,
                FeatureLevel.Level_11_1);

            // Don't know why but creating a separate device solves AccessViolationExceptions happening otherwise
            _deviceForDeskDupl = new Device(Output.GetParent<Adapter>());

            _duplCapture = new DuplCapture(_deviceForDeskDupl, Output);

            _stagingTexture = new Texture2D(_device, new Texture2DDescription
            {
                CpuAccessFlags = CpuAccessFlags.Read,
                BindFlags = BindFlags.None,
                Format = Format.B8G8R8A8_UNorm,
                Width = Rect.Width,
                Height = Rect.Height,
                OptionFlags = ResourceOptionFlags.None,
                MipLevels = 1,
                ArraySize = 1,
                SampleDescription = { Count = 1, Quality = 0 },
                Usage = ResourceUsage.Staging
            });

            _desktopImageTexture = new Texture2D(_device, new Texture2DDescription
            {
                CpuAccessFlags = CpuAccessFlags.None,
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                Format = Format.B8G8R8A8_UNorm,
                Width = Rect.Width,
                Height = Rect.Height,
                OptionFlags = ResourceOptionFlags.None,
                MipLevels = 1,
                ArraySize = 1,
                SampleDescription = { Count = 1, Quality = 0 },
                Usage = ResourceUsage.Default
            });

            _editorSession = new Direct2DEditorSession(_desktopImageTexture, _device, _stagingTexture);

            if (IncludeCursor)
                _mousePointer = new DxMousePointer(_editorSession);
        }
        
        public IEditableFrame Capture()
        {
            try
            {
                if (!_duplCapture.Get(_desktopImageTexture, _mousePointer))
                    return RepeatFrame.Instance;
            }
            catch
            {
                try { _duplCapture.Init(); }
                catch
                {
                    // ignored
                }

                return RepeatFrame.Instance;
            }

            var editor = new Direct2DEditor(_editorSession);

            _mousePointer?.Draw(editor);

            return editor;
        }

        public void Dispose()
        {
            try { _mousePointer?.Dispose(); }
            catch { }

            try { _editorSession.Dispose(); }
            catch { }

            try { _duplCapture.Dispose(); }
            catch { }

            try { _desktopImageTexture?.Dispose(); }
            catch { }

            try { _stagingTexture?.Dispose(); }
            catch { }

            try { _device?.Dispose(); }
            catch { }

            try { _deviceForDeskDupl?.Dispose(); }
            catch { }
        }
    }
}
