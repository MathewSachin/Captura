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
        readonly int _width, _height;

        // Resources can only be used with the device that created them.
        // Frames come from the duplication device, so they reach the capture device through a shared texture.
        Texture2D _sharedTexture, _targetTexture;
        KeyedMutex _sharedMutex, _targetMutex;
        IntPtr _targetDevicePointer;
        bool _hasFrame;

        const int MutexTimeout = 1000;

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

                DisposeSharedTexture();

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

        void CreateSharedTexture(Device TargetDevice)
        {
            DisposeSharedTexture();

            _sharedTexture = new Texture2D(_device, new Texture2DDescription
            {
                CpuAccessFlags = CpuAccessFlags.None,
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                Format = Format.B8G8R8A8_UNorm,
                Width = _width,
                Height = _height,
                OptionFlags = ResourceOptionFlags.SharedKeyedmutex,
                MipLevels = 1,
                ArraySize = 1,
                SampleDescription = { Count = 1, Quality = 0 },
                Usage = ResourceUsage.Default
            });

            _sharedMutex = _sharedTexture.QueryInterface<KeyedMutex>();

            using (var resource = _sharedTexture.QueryInterface<SharpDX.DXGI.Resource>())
                _targetTexture = TargetDevice.OpenSharedResource<Texture2D>(resource.SharedHandle);

            _targetMutex = _targetTexture.QueryInterface<KeyedMutex>();
            _targetDevicePointer = TargetDevice.NativePointer;
        }

        void DisposeSharedTexture()
        {
            _targetMutex?.Dispose();
            _targetTexture?.Dispose();
            _sharedMutex?.Dispose();
            _sharedTexture?.Dispose();

            _targetMutex = _sharedMutex = null;
            _targetTexture = _sharedTexture = null;
            _hasFrame = false;
        }

        bool Acquire(KeyedMutex Mutex)
        {
            if (Mutex.Acquire(0, MutexTimeout) == Result.Ok)
                return true;

            // An abandoned keyed mutex can't be reused, so start over with a new shared texture
            DisposeSharedTexture();

            return false;
        }

        public bool Get(Texture2D Texture, DxMousePointer DxMousePointer, Point TargetPosition = default)
        {
            lock (_syncLock)
            {
                // Disposed
                if (_device == null)
                    return false;

                var targetDevice = Texture.Device;

                if (_targetTexture == null || _targetDevicePointer != targetDevice.NativePointer)
                    CreateSharedTexture(targetDevice);

                var acquireResult = _frameGrabber.Grab();

                if (acquireResult != null && acquireResult.Result.Failure)
                {
                    throw new Exception($"Failed to acquire next frame: {acquireResult.Result.Code}");
                }

                if (acquireResult == null && !_hasFrame)
                    return false;

                try
                {
                    if (acquireResult != null)
                    {
                        using (acquireResult.DesktopResource)
                        using (var tempTexture = acquireResult.DesktopResource.QueryInterface<Texture2D>())
                        {
                            if (!Acquire(_sharedMutex))
                                return false;

                            try
                            {
                                _device.ImmediateContext.CopyResource(tempTexture, _sharedTexture);
                                _hasFrame = true;
                            }
                            finally
                            {
                                _sharedMutex.Release(0);
                            }
                        }
                    }

                    if (!Acquire(_targetMutex))
                        return false;

                    try
                    {
                        if (acquireResult != null)
                            DxMousePointer?.Update(_targetTexture, acquireResult.FrameInfo, _deskDupl);

                        targetDevice.ImmediateContext.CopySubresourceRegion(_targetTexture,
                            0,
                            new ResourceRegion(0, 0, 0, _width, _height, 1),
                            Texture,
                            0,
                            TargetPosition.X, TargetPosition.Y);
                    }
                    finally
                    {
                        _targetMutex.Release(0);
                    }

                    return true;
                }
                finally
                {
                    // After the pointer update: the pointer shape can only be read while the frame is held
                    if (acquireResult != null)
                        _frameGrabber.Release();
                }
            }
        }
    }
}