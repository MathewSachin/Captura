// Adapted from https://github.com/jasonpang/desktop-duplication-net

using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using Captura;
using SharpDX.Direct3D;
using Device = SharpDX.Direct3D11.Device;

namespace DesktopDuplication
{
    public class DesktopDuplicator : IDisposable
    {
        Texture2D _desktopImageTexture;
        Texture2D _stagingTexture;
        Direct2DEditorSession _editorSession;
        DxMousePointer _mousePointer;
        DuplCapture _duplCapture;
        Device _device;
        Device _deviceForDeskDupl;

        public DesktopDuplicator(bool IncludeCursor, Output1 Output)
        {
            _device = new Device(DriverType.Hardware,
                DeviceCreationFlags.BgraSupport,
                FeatureLevel.Level_11_1);

            // Don't know why but creating a separate device solves AccessViolationExceptions happening otherwise
            _deviceForDeskDupl = new Device(Output.GetParent<Adapter>());

            _duplCapture = new DuplCapture(_deviceForDeskDupl, Output);

            var bounds = Output.Description.DesktopBounds;
            var width = bounds.Right - bounds.Left;
            var height = bounds.Bottom - bounds.Top;

            _stagingTexture = new Texture2D(_device, new Texture2DDescription
            {
                CpuAccessFlags = CpuAccessFlags.Read,
                BindFlags = BindFlags.None,
                Format = Format.B8G8R8A8_UNorm,
                Width = width,
                Height = height,
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
                Width = width,
                Height = height,
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
            finally { _mousePointer = null; }

            try { _editorSession.Dispose(); }
            catch { }
            finally { _editorSession = null; }

            try { _duplCapture.Dispose(); }
            catch { }
            finally { _duplCapture = null; }

            try { _desktopImageTexture.Dispose(); }
            catch { }
            finally { _desktopImageTexture = null; }

            try { _stagingTexture.Dispose(); }
            catch { }
            finally { _stagingTexture = null; }

            try { _device.Dispose(); }
            catch { }
            finally { _device = null; }

            try { _deviceForDeskDupl.Dispose(); }
            catch { }
            finally { _deviceForDeskDupl = null; }
        }
    }
}
