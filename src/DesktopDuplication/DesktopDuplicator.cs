// Adapted from https://github.com/jasonpang/desktop-duplication-net

using Screna;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using Captura;
using Device = SharpDX.Direct3D11.Device;
using MapFlags = SharpDX.Direct3D11.MapFlags;

namespace DesktopDuplication
{
    public class DesktopDuplicator : IDisposable
    {
        #region Fields
        readonly Device _device;
        readonly OutputDuplication _deskDupl;

        readonly Texture2D _desktopImageTexture;
        OutputDuplicateFrameInformation _frameInfo;

        Rectangle _rect;

        readonly bool _includeCursor;
        #endregion

        public int Timeout { get; set; }

        public DesktopDuplicator(Rectangle Rect, bool IncludeCursor, Adapter1 Adapter, Output1 Output)
        {
            _rect = Rect;
            _includeCursor = IncludeCursor;
            
            _device = new Device(Adapter);

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
                SampleDescription = { Count = 1, Quality = 0 },
                Usage = ResourceUsage.Staging
            };

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

            _desktopImageTexture = new Texture2D(_device, textureDesc);
        }
        
        public IBitmapFrame Capture()
        {
            SharpDX.DXGI.Resource desktopResource;

            try
            {
                _deskDupl.AcquireNextFrame(Timeout, out _frameInfo, out desktopResource);
            }
            catch (SharpDXException e) when (e.Descriptor == SharpDX.DXGI.ResultCode.WaitTimeout)
            {
                return RepeatFrame.Instance;
            }
            catch (SharpDXException e) when (e.ResultCode.Failure)
            {
                throw new Exception("Failed to acquire next frame.", e);
            }
            
            using (desktopResource)
            {
                using (var tempTexture = desktopResource.QueryInterface<Texture2D>())
                {
                    var resourceRegion = new ResourceRegion(_rect.Left, _rect.Top, 0, _rect.Right, _rect.Bottom, 1);

                    _device.ImmediateContext.CopySubresourceRegion(tempTexture, 0, resourceRegion, _desktopImageTexture, 0);
                }
            }

            ReleaseFrame();

            var mapSource = _device.ImmediateContext.MapSubresource(_desktopImageTexture, 0, MapMode.Read, MapFlags.None);

            try
            {
                return new OneTimeFrame(ProcessFrame(mapSource.DataPointer, mapSource.RowPitch));
            }
            finally
            {
                _device.ImmediateContext.UnmapSubresource(_desktopImageTexture, 0);
            }
        }

        Bitmap ProcessFrame(IntPtr SourcePtr, int SourceRowPitch)
        {
            var frame = new Bitmap(_rect.Width, _rect.Height, PixelFormat.Format32bppRgb);

            // Copy pixels from screen capture Texture to GDI bitmap
            var mapDest = frame.LockBits(new Rectangle(0, 0, _rect.Width, _rect.Height), ImageLockMode.WriteOnly, frame.PixelFormat);

            Parallel.For(0, _rect.Height, Y =>
            {
                Utilities.CopyMemory(mapDest.Scan0 + Y * mapDest.Stride,
                    SourcePtr + Y * SourceRowPitch,
                    _rect.Width * 4);
            });

            // Release source and dest locks
            frame.UnlockBits(mapDest);

            if (_includeCursor && (_frameInfo.LastMouseUpdateTime == 0 || _frameInfo.PointerPosition.Visible))
            {
                using (var g = Graphics.FromImage(frame))
                    MouseCursor.Draw(g, P => new Point(P.X - _rect.X, P.Y - _rect.Y));
            }

            return frame;
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
                _desktopImageTexture?.Dispose();
                _device?.Dispose();
            }
            catch { }
        }
    }
}
