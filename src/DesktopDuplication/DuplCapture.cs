using System;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D11.Device;

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
            _deskDupl?.Dispose();

            try
            {
                _deskDupl = _output.DuplicateOutput(_device);
            }
            catch (SharpDXException e) when (e.Descriptor == SharpDX.DXGI.ResultCode.NotCurrentlyAvailable)
            {
                throw new Exception("There is already the maximum number of applications using the Desktop Duplication API running, please close one of the applications and try again.", e);
            }
            catch (SharpDXException e) when (e.Descriptor == SharpDX.DXGI.ResultCode.Unsupported)
            {
                throw new NotSupportedException("Desktop Duplication is not supported on this system.\nIf you have multiple graphic cards, try running Captura on integrated graphics.", e);
            }
        }

        public OutputDuplicateFrameInformation? Get(Texture2D Texture, int Timeout, DxMousePointer DxMousePointer)
        {
            SharpDX.DXGI.Resource desktopResource;
            OutputDuplicateFrameInformation frameInfo;

            try
            {
                _deskDupl.AcquireNextFrame(Timeout, out frameInfo, out desktopResource);
            }
            catch (SharpDXException e) when (e.Descriptor == SharpDX.DXGI.ResultCode.WaitTimeout)
            {
                return null;
            }
            catch (SharpDXException e) when (e.ResultCode.Failure)
            {
                throw new Exception("Failed to acquire next frame.", e);
            }

            using (desktopResource)
            {
                using (var tempTexture = desktopResource.QueryInterface<Texture2D>())
                {
                    _device.ImmediateContext.CopyResource(tempTexture, Texture);
                }
            }

            DxMousePointer?.Update(frameInfo, _deskDupl);

            ReleaseFrame();

            return frameInfo;
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