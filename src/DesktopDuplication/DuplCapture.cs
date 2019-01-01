using System;
using System.Threading.Tasks;
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
        readonly Device _device;
        OutputDuplication _deskDupl;

        readonly object _syncLock = new object();

        public DuplCapture(Device Device, Output1 Output)
        {
            _device = Device;
            _output = Output;

            Init();
        }

        public void Dispose()
        {
            try { _acquireTask?.Wait(); }
            catch { }

            _deskDupl?.Dispose();
        }

        public void Init()
        {
            lock (_syncLock)
            {
                try
                {
                    _acquireTask?.Wait();
                }
                catch { }

                _acquireTask = null;
                _deskDupl?.Dispose();

                try
                {
                    _deskDupl = _output.DuplicateOutput(_device);
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

        Task<AcquireResult> _acquireTask;

        void BeginAcquireFrame()
        {
            const int timeout = 5000;

            _acquireTask = Task.Run(() =>
            {
                try
                {
                    var result = _deskDupl.TryAcquireNextFrame(timeout, out var frameInfo, out var desktopResource);

                    return new AcquireResult(result, frameInfo, desktopResource);
                }
                catch
                {
                    return new AcquireResult(Result.Fail);
                }
            });
        }

        public bool Get(Texture2D Texture, DxMousePointer DxMousePointer)
        {
            lock (_syncLock)
            {
                if (_acquireTask == null)
                {
                    BeginAcquireFrame();

                    return false;
                }

                var acquireResult = _acquireTask.Result;

                if (acquireResult.Result == ResultCode.WaitTimeout)
                {
                    BeginAcquireFrame();

                    return false;
                }

                if (acquireResult.Result.Failure)
                {
                    throw new Exception($"Failed to acquire next frame: {acquireResult.Result.Code}");
                }

                DxMousePointer?.Update(acquireResult.FrameInfo, _deskDupl);

                using (acquireResult.DesktopResource)
                using (var tempTexture = acquireResult.DesktopResource.QueryInterface<Texture2D>())
                {
                    _device.ImmediateContext.CopyResource(tempTexture, Texture);
                }

                _deskDupl.ReleaseFrame();

                BeginAcquireFrame();

                return true;
            }
        }
    }
}