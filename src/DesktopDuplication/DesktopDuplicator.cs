// Adapted from https://github.com/jasonpang/desktop-duplication-net

using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Drawing;
using System.Drawing.Imaging;
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
        #endregion

        public DesktopDuplicator(int Monitor, int Adapter = 0)
        {
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
                Width = _outputDesc.DesktopBounds.Width,
                Height = _outputDesc.DesktopBounds.Height,
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

        public Bitmap GetLatestFrame(DRectangle Rect)
        {
            // Try to get the latest frame; this may timeout
            if (!RetrieveFrame())
                return null;

            try
            {
                return ProcessFrame(Rect);
            }
            finally
            {
                ReleaseFrame();
            }
        }

        /// <summary>
        /// Returns true on success, false on timeout
        /// </summary>
        bool RetrieveFrame()
        {
            if (_desktopImageTexture == null)
                _desktopImageTexture = new Texture2D(_device, _textureDesc);
                        
            try
            {
                _deskDupl.AcquireNextFrame(500, out _frameInfo, out var desktopResource);

                using (var tempTexture = desktopResource.QueryInterface<Texture2D>())
                    _device.ImmediateContext.CopyResource(tempTexture, _desktopImageTexture);

                desktopResource.Dispose();

                return true;
            }
            catch (SharpDXException e)
            {
                if (e.ResultCode.Code == SharpDX.DXGI.ResultCode.WaitTimeout.Result.Code)
                {
                    return false;
                }

                if (e.ResultCode.Failure)
                {
                    throw new Exception("Failed to acquire next frame.", e);
                }

                throw;
            }
        }
        
        Bitmap ProcessFrame(DRectangle Rect)
        {
            // Get the desktop capture texture
            var mapSource = _device.ImmediateContext.MapSubresource(_desktopImageTexture, 0, MapMode.Read, MapFlags.None);

            var image = new Bitmap(Rect.Width, Rect.Height, PixelFormat.Format32bppRgb);

            // Copy pixels from screen capture Texture to GDI bitmap
            var mapDest = image.LockBits(new DRectangle(0, 0, Rect.Width, Rect.Height), ImageLockMode.ReadWrite, image.PixelFormat);

            var srcPtr = mapSource.DataPointer + Rect.X * 4 + mapSource.RowPitch * Rect.Y;
            var destPtr = mapDest.Scan0;

            for (int y = 0; y < Rect.Height; ++y)
            {
                Utilities.CopyMemory(destPtr, srcPtr, Rect.Width * 4);

                srcPtr += mapSource.RowPitch;
                destPtr += mapDest.Stride;
            }

            // Release source and dest locks
            image.UnlockBits(mapDest);
            _device.ImmediateContext.UnmapSubresource(_desktopImageTexture, 0);
            return image;
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
            _deskDupl?.Dispose();
            _desktopImageTexture?.Dispose();
            _device?.Dispose();
        }
    }
}
