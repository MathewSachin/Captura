using System;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D11.Device;
using Resource = SharpDX.DXGI.Resource;
using ResultCode = SharpDX.DXGI.ResultCode;

namespace DesktopDuplication
{
    public class DuplCapture : IDisposable
    {
        readonly Output1 _output;
        readonly Device _device;
        OutputDuplication _deskDupl;

        public DuplCapture(Device Device, Output1 Output)
        {
            _device = Device;
            _output = Output;

            Init();
        }

        public void Dispose()
        {
            _deskDupl?.Dispose();
        }

        public void Init()
        {
            _acquireResult = null;
            _deskDupl?.Dispose();

            try
            {
                _deskDupl = _output.DuplicateOutput(_device);
            }
            catch (SharpDXException e) when (e.Descriptor == ResultCode.NotCurrentlyAvailable)
            {
                throw new Exception("There is already the maximum number of applications using the Desktop Duplication API running, please close one of the applications and try again.", e);
            }
            catch (SharpDXException e) when (e.Descriptor == ResultCode.Unsupported)
            {
                throw new NotSupportedException("Desktop Duplication is not supported on this system.\nIf you have multiple graphic cards, try running Captura on integrated graphics.", e);
            }
        }

        Result? _acquireResult;
        OutputDuplicateFrameInformation _frameInfo;
        Resource _desktopResource;

        void AcquireFrame()
        {
            const int timeout = 5000;

            _acquireResult = _deskDupl.TryAcquireNextFrame(timeout, out _frameInfo, out _desktopResource);
        }

        public OutputDuplicateFrameInformation? Get(Texture2D Texture, int Timeout, DxMousePointer DxMousePointer)
        {
            if (_acquireResult == null)
            {
                AcquireFrame();

                return null;
            }

            if (_acquireResult == ResultCode.WaitTimeout)
            {
                return null;
            }

            if (_acquireResult.Value.Failure)
            {
                throw new Exception($"Failed to acquire next frame: {_acquireResult.Value.Code}");
            }

            using (_desktopResource)
            {
                using (var tempTexture = _desktopResource.QueryInterface<Texture2D>())
                {
                    _device.ImmediateContext.CopyResource(tempTexture, Texture);
                }
            }

            DxMousePointer?.Update(_frameInfo, _deskDupl);

            ReleaseFrame();

            AcquireFrame();

            return _frameInfo;
        }

        void ReleaseFrame()
        {
            try
            {
                _deskDupl.ReleaseFrame();
            }
            catch (SharpDXException e)
            {
                if (e.ResultCode.Failure)
                {
                    throw new Exception("Failed to release frame.", e);
                }
            }
        }
    }
}