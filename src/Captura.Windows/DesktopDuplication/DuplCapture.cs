using System;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D11.Device;
using ResultCode = SharpDX.DXGI.ResultCode;

namespace Captura.Windows.DesktopDuplication
{
    public class DuplCapture : IDisposable
    {
        readonly Output1 _output;
        Device _device;
        OutputDuplication _deskDupl;
        FrameGrabber _frameGrabber;
        Texture2D _bkpTexture;
        readonly int _width, _height;

        readonly object _syncLock = new object();

        public DuplCapture(Output1 Output)
        {
            // Separate Device required otherwise AccessViolationException happens
            using (var adapter = Output.GetParent<Adapter>())
                _device = new Device(adapter);

            _output = Output;

            var bound = Output.Description.DesktopBounds;
            _width = bound.Right - bound.Left;
            _height = bound.Bottom - bound.Top;

            Init();
        }

        public void Dispose()
        {
            lock (_syncLock)
            {
                _frameGrabber?.Dispose();

                _deskDupl?.Dispose();

                _bkpTexture?.Dispose();

                _device.Dispose();
                _device = null;
            }
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

        public bool Get(Texture2D Texture, DxMousePointer DxMousePointer, Point TargetPosition = default)
        {
            lock (_syncLock)
            {
                // Disposed
                if (_device == null)
                    return false;

                if (_bkpTexture == null)
                {
                    var desc = Texture.Description;
                    desc.Width = _width;
                    desc.Height = _height;

                    // _device is being used by Desktop Duplication
                    _bkpTexture = new Texture2D(Texture.Device, desc);
                }

                var acquireResult = _frameGrabber.Grab();

                if (acquireResult == null)
                {
                    // _device is being used by Desktop Duplication
                    Texture.Device.ImmediateContext.CopySubresourceRegion(_bkpTexture,
                        0,
                        new ResourceRegion(0, 0, 0, _width, _height, 1),
                        Texture,
                        0,
                        TargetPosition.X, TargetPosition.Y);

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

                    Texture.Device.ImmediateContext.CopySubresourceRegion(tempTexture,
                        0,
                        new ResourceRegion(0, 0, 0, _width, _height, 1),
                        Texture,
                        0,
                        TargetPosition.X, TargetPosition.Y);

                    _device.ImmediateContext.CopyResource(tempTexture, _bkpTexture);
                }

                _frameGrabber.Release();

                return true;
            }
        }
    }
}