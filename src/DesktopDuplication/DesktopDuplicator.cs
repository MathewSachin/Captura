// Adapted from https://github.com/jasonpang/desktop-duplication-net

using Screna;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using Device = SharpDX.Direct3D11.Device;
using DRectangle = System.Drawing.Rectangle;
using MapFlags = SharpDX.Direct3D11.MapFlags;

namespace DesktopDuplication
{
    public class DesktopDuplicator : IDisposable
    {
        #region Fields
        readonly Device _device;
        readonly Texture2DDescription _textureDesc;
        OutputDescription _outputDesc;
        readonly OutputDuplication _deskDupl;

        Texture2D _desktopImageTexture;
        OutputDuplicateFrameInformation _frameInfo;

        DRectangle _rect;

        readonly bool _includeCursor;
        #endregion

        public int Timeout { get; set; }

        public DesktopDuplicator(DRectangle Rect, bool IncludeCursor, int Monitor, int Adapter = 0)
        {
            _rect = Rect;
            _includeCursor = IncludeCursor;

            Adapter1 adapter;
            try
            {
                adapter = new Factory1().GetAdapter1(Adapter);
            }
            catch (SharpDXException e)
            {
                throw new Exception("Could not find the specified graphics card adapter.", e);
            }

            _device = new Device(adapter);

            Output output;
            try
            {
                output = adapter.GetOutput(Monitor);
            }
            catch (SharpDXException e)
            {
                throw new Exception("Could not find the specified output device.", e);
            }

            var output1 = output.QueryInterface<Output1>();
            _outputDesc = output.Description;
            
            _textureDesc = new Texture2DDescription
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
                _deskDupl = output1.DuplicateOutput(_device);
            }
            catch (SharpDXException e)
            {
                if (e.ResultCode.Code == SharpDX.DXGI.ResultCode.NotCurrentlyAvailable.Result.Code)
                {
                    throw new Exception("There is already the maximum number of applications using the Desktop Duplication API running, please close one of the applications and try again.", e);
                }
            }
        }
        
        Bitmap _lastFrame;

        public Bitmap Capture()
        {
            if (_desktopImageTexture == null)
                _desktopImageTexture = new Texture2D(_device, _textureDesc);

            SharpDX.DXGI.Resource desktopResource;

            try
            {
                _deskDupl.AcquireNextFrame(Timeout, out _frameInfo, out desktopResource);
            }
            catch (SharpDXException e) when (e.ResultCode.Code == SharpDX.DXGI.ResultCode.WaitTimeout.Result.Code)
            {
                return _lastFrame ?? new Bitmap(_rect.Width, _rect.Height);
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
                return ProcessFrame(mapSource.DataPointer, mapSource.RowPitch);
            }
            finally
            {
                _device.ImmediateContext.UnmapSubresource(_desktopImageTexture, 0);
            }
        }

        Bitmap ProcessFrame(IntPtr SourcePtr, int SourceRowPitch)
        {
            _lastFrame = new Bitmap(_rect.Width, _rect.Height, PixelFormat.Format32bppRgb);

            // Copy pixels from screen capture Texture to GDI bitmap
            var mapDest = _lastFrame.LockBits(new DRectangle(0, 0, _rect.Width, _rect.Height), ImageLockMode.WriteOnly, _lastFrame.PixelFormat);

            Parallel.For(0, _rect.Height, y =>
            {
                Utilities.CopyMemory(mapDest.Scan0 + y * mapDest.Stride,
                    SourcePtr + y * SourceRowPitch,
                    _rect.Width * 4);
            });
                        
            // Release source and dest locks
            _lastFrame.UnlockBits(mapDest);

            if (_includeCursor && _frameInfo.PointerPosition.Visible)
            {
                using (var g = Graphics.FromImage(_lastFrame))
                    MouseCursor.Draw(g, P => new System.Drawing.Point(P.X - _rect.X, P.Y - _rect.Y));
            }

            return _lastFrame;
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
