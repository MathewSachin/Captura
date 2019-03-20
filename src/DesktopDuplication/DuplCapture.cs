using System;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D11.Device;
using ResultCode = SharpDX.DXGI.ResultCode;

namespace DesktopDuplication
{
    public class DuplCapture : IDisposable
    {
        readonly Output1 _output;
        Device _device;
        OutputDuplication _deskDupl;
        FrameGrabber _frameGrabber;
        Texture2D _bkpTexture;

        readonly object _syncLock = new object();

        public DuplCapture(Output1 Output)
        {
            // Separate Device required otherwise AccessViolationException happens
            using (var adapter = Output.GetParent<Adapter>())
                _device = new Device(adapter);

            _output = Output;

            Init();
        }

        public void Dispose()
        {
            _frameGrabber?.Dispose();

            _deskDupl?.Dispose();

            _bkpTexture?.Dispose();

            _device.Dispose();
            _device = null;
        }

        public void Init()
        {
            lock (_syncLock)
            {
                _frameGrabber?.Dispose();
                _deskDupl?.Dispose();

                try
                {
                    _deskDupl = _output.DuplicateOutput(_device);

                    _frameGrabber = new FrameGrabber(_deskDupl);
                }
                catch (SharpDXException e) when (e.Descriptor == ResultCode.NotCurrentlyAvailable)
                {
                    throw new Exception(
                        "There is already the maximum number of applications using the Desktop Duplication API running, please close one of the applications and try again.",
                        e);
                }
                catch (SharpDXException e) when (e.Descriptor == ResultCode.Unsupported)
                {
                    throw new NotSupportedException(
                        "Desktop Duplication is not supported on this system.\nIf you have multiple graphic cards, try running Captura on integrated graphics.",
                        e);
                }
            }
        }

        public bool Get(Texture2D Texture, DxMousePointer DxMousePointer)
        {
            lock (_syncLock)
            {
                if (_bkpTexture == null)
                {
                    // _device is being used by Desktop Duplication
                    _bkpTexture = new Texture2D(Texture.Device, Texture.Description);
                }

                var acquireResult = _frameGrabber.Grab();

                if (acquireResult == null)
                {
                    // _device is being used by Desktop Duplication
                    Texture.Device.ImmediateContext.CopyResource(_bkpTexture, Texture);

                    return true;
                }

                if (acquireResult.Result.Failure)
                {
                    throw new Exception($"Failed to acquire next frame: {acquireResult.Result.Code}");
                }

                using (acquireResult.DesktopResource)
                using (var tempTexture = acquireResult.DesktopResource.QueryInterface<Texture2D>())
                {
                    DxMousePointer?.Update(tempTexture, acquireResult.FrameInfo, _deskDupl);

                    _device.ImmediateContext.CopyResource(tempTexture, Texture);
                    _device.ImmediateContext.CopyResource(tempTexture, _bkpTexture);
                }

                _frameGrabber.Release();

                return true;
            }
        }
    }
}