// Adapted from https://github.com/jasonpang/desktop-duplication-net

using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using Device = SharpDX.Direct3D11.Device;
using Rectangle = System.Drawing.Rectangle;

namespace DesktopDuplication
{
    public class DeskDuplMediaFoundation : IDisposable
    {
        #region Fields
        readonly Device _device;
        readonly OutputDuplication _deskDupl;
        OutputDuplicateFrameInformation _frameInfo;

        Rectangle _rect;

        readonly bool _includeCursor;
        #endregion

        public int Timeout { get; set; }

        readonly TextureAllocator _textureAllocator;
        readonly MfWriter _writer;

        public DeskDuplMediaFoundation(Rectangle Rect, bool IncludeCursor, Adapter Adapter, Output1 Output)
        {
            MfManager.Startup();

            _rect = Rect;
            _includeCursor = IncludeCursor;
            
            _device = new Device(Adapter);
            _writer = new MfWriter(_device, 30, _rect.Width, _rect.Height);

            var textureDesc = new Texture2DDescription
            {
                CpuAccessFlags = CpuAccessFlags.Read,
                BindFlags = BindFlags.None,
                Format = Format.B8G8R8A8_UNorm,
                Width = _rect.Width,
                Height = _rect.Height,
                OptionFlags = ResourceOptionFlags.None,
                MipLevels = 1,
                ArraySize = 1,
                SampleDescription = {Count = 1, Quality = 0},
                Usage = ResourceUsage.Staging
            };

            _textureAllocator = new TextureAllocator(textureDesc, _device);
            
            try
            {
                _deskDupl = Output.DuplicateOutput(_device);
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
            SharpDX.DXGI.Resource desktopResource;

            try
            {
                _deskDupl.AcquireNextFrame(Timeout, out _frameInfo, out desktopResource);
            }
            catch (SharpDXException e) when (e.Descriptor == SharpDX.DXGI.ResultCode.WaitTimeout)
            {
                return;
            }
            catch (SharpDXException e) when (e.ResultCode.Failure)
            {
                throw new Exception("Failed to acquire next frame.", e);
            }

            var texture = _textureAllocator.AllocateTexture();
            
            using (desktopResource)
            {
                using (var tempTexture = desktopResource.QueryInterface<Texture2D>())
                {
                    var resourceRegion = new ResourceRegion(_rect.Left, _rect.Top, 0, _rect.Right, _rect.Bottom, 1);

                    _device.ImmediateContext.CopySubresourceRegion(tempTexture, 0, resourceRegion, texture, 0);
                }
            }

            ReleaseFrame();

            var sample = _textureAllocator.CreateSample(texture);

            _writer.Write(sample);
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

        public void Dispose()
        {
            try
            {
                _deskDupl?.Dispose();
                _device?.Dispose();

                _textureAllocator.Dispose();

                MfManager.Shutdown();
            }
            catch { }
        }
    }
}
