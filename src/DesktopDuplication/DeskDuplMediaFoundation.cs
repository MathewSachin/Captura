// Adapted from https://github.com/jasonpang/desktop-duplication-net

using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Threading.Tasks;
using SharpDX.Direct3D;
using Device = SharpDX.Direct3D11.Device;
using Rectangle = System.Drawing.Rectangle;
using Resource = SharpDX.DXGI.Resource;

namespace DesktopDuplication
{
    public class DeskDuplMediaFoundation : IDisposable
    {
        #region Fields
        readonly Device _device;
        readonly Output1 _output;
        OutputDuplication _deskDupl;

        Rectangle _rect;
        #endregion

        public int Timeout { get; set; }

        readonly TextureAllocator _textureAllocator;
        readonly MfWriter _writer;

        public int Fps { get; }

        DateTime _lastAcquireTime;
        readonly int _frameIntervalMs;

        public DeskDuplMediaFoundation(Rectangle Rect, Adapter Adapter, Output1 Output, int Fps, string FileName)
        {
            _rect = Rect;
            _output = Output;
            this.Fps = Fps;

            _frameIntervalMs = 1000 / Fps;
            
            _device = new Device(Adapter, DeviceCreationFlags.VideoSupport);
            _writer = new MfWriter(_device, Fps, _rect.Width, _rect.Height, FileName);

            var textureDesc = new Texture2DDescription
            {
                CpuAccessFlags = CpuAccessFlags.None,
                BindFlags = BindFlags.None,
                Format = Format.B8G8R8A8_UNorm,
                Width = _rect.Width,
                Height = _rect.Height,
                OptionFlags = ResourceOptionFlags.None,
                MipLevels = 1,
                ArraySize = 1,
                SampleDescription = {Count = 1, Quality = 0}
            };

            _textureAllocator = new TextureAllocator(textureDesc, _device);
            
            ReInit();
        }

        void ReInit()
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
        
        public void Capture()
        {
            if (!AcquireFrame(out var desktopResource))
                return;

            var allocatedTexture = _textureAllocator.AllocateTexture();
            
            using (desktopResource)
            {
                using (var tempTexture = desktopResource.QueryInterface<Texture2D>())
                {
                    var resourceRegion = new ResourceRegion(_rect.Left, _rect.Top, 0, _rect.Right, _rect.Bottom, 1);

                    _device.ImmediateContext.CopySubresourceRegion(tempTexture, 0, resourceRegion, allocatedTexture.Texture, 0);
                }
            }

            lock (_syncLock)
            {
                if (!_disposed)
                    ReleaseFrame();
            }

            _writeTask?.Wait();

            _writeTask = Task.Run(() =>
            {
                _writer.Write(allocatedTexture.Sample);
            });
        }

        Task _writeTask;

        bool AcquireFrame(out Resource DesktopResource)
        {
            DesktopResource = null;

            try
            {
                lock (_syncLock)
                {
                    while (!_disposed)
                    {
                        _deskDupl.AcquireNextFrame(Timeout, out var frameInfo, out DesktopResource);

                        var now = DateTime.Now;

                        var diff = (now - _lastAcquireTime).TotalMilliseconds;

                        // Drop Frame
                        if (diff < _frameIntervalMs / 4.0)
                        {
                            ReleaseFrame();
                        }
                        else
                        {
                            _lastAcquireTime = now;

                            return true;
                        }
                    }
                }

                return false;
            }
            catch (SharpDXException e) when (e.Descriptor == SharpDX.DXGI.ResultCode.WaitTimeout)
            {
                return false;
            }
            catch (SharpDXException e) when (e.ResultCode.Failure)
            {
                lock (_syncLock)
                    if (!_disposed)
                        ReInit();

                return false;
                //throw new Exception("Failed to acquire next frame.", e);
            }
            catch (NullReferenceException)
            {
                // Happens on end
                return false;
            }
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

        bool _disposed;

        readonly object _syncLock = new object();

        public void Dispose()
        {
            try
            {
                lock (_syncLock)
                {
                    if (_disposed)
                        return;

                    _disposed = true;

                    _deskDupl?.Dispose();

                    _writeTask?.Wait();

                    _device?.Dispose();

                    _writer.Dispose();

                    _textureAllocator.Dispose();
                }
            }
            catch { }
        }
    }
}
