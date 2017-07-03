// Adapted from https://github.com/jasonpang/desktop-duplication-net

using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
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

        public Bitmap GetLatestFrame(DRectangle? Rect = null)
        {
            // Try to get the latest frame; this may timeout
            if (!RetrieveFrame())
                return null;

            try
            {
                if (Rect == null)
                {
                    // Moved and Updated Regions are used only for full capture.
                    RetrieveFrameMetadata();

                    return ProcessFrame();
                }
                else return ProcessFrame(Rect.Value);
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

            SharpDX.DXGI.Resource desktopResource = null;

            _frameInfo = new OutputDuplicateFrameInformation();

            try
            {
                _deskDupl.AcquireNextFrame(500, out _frameInfo, out desktopResource);
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
            }

            using (var tempTexture = desktopResource.QueryInterface<Texture2D>())
                _device.ImmediateContext.CopyResource(tempTexture, _desktopImageTexture);

            desktopResource.Dispose();

            return true;
        }

        MovedRegion[] MovedRegions;
        DRectangle[] UpdatedRegions;

        OutputDuplicateMoveRectangle[] _moveBuffer;
        SharpDX.Rectangle[] _updateBuffer;

        // Information of Moved and Updated Regions
        void RetrieveFrameMetadata()
        {
            if (_frameInfo.TotalMetadataBufferSize > 0)
            {
                // Get moved regions
                if (_moveBuffer == null || _moveBuffer.Length < _frameInfo.TotalMetadataBufferSize)
                    _moveBuffer = new OutputDuplicateMoveRectangle[_frameInfo.TotalMetadataBufferSize];

                _deskDupl.GetFrameMoveRects(_moveBuffer.Length, _moveBuffer, out int movedRegionsLength);

                MovedRegions = new MovedRegion[movedRegionsLength / Marshal.SizeOf<OutputDuplicateMoveRectangle>()];

                for (int i = 0; i < MovedRegions.Length; i++)
                {
                    MovedRegions[i] = new MovedRegion
                    {
                        Source = _moveBuffer[i].SourcePoint.ToDPoint(),
                        Destination = _moveBuffer[i].DestinationRect.ToDRect()
                    };
                }

                // Get dirty regions
                if (_updateBuffer == null || _updateBuffer.Length < _frameInfo.TotalMetadataBufferSize)
                    _updateBuffer = new SharpDX.Rectangle[_frameInfo.TotalMetadataBufferSize];

                _deskDupl.GetFrameDirtyRects(_updateBuffer.Length, _updateBuffer, out int dirtyRegionsLength);

                UpdatedRegions = new DRectangle[dirtyRegionsLength / Marshal.SizeOf<SharpDX.Rectangle>()];

                for (int i = 0; i < UpdatedRegions.Length; i++)
                {
                    UpdatedRegions[i] = _updateBuffer[i].ToDRect();
                }
            }
            else
            {
                MovedRegions = new MovedRegion[0];
                UpdatedRegions = new DRectangle[0];
            }
        }

        Bitmap image;

        // Capture full
        Bitmap ProcessFrame(DRectangle Rect)
        {
            // Get the desktop capture texture
            var mapSource = _device.ImmediateContext.MapSubresource(_desktopImageTexture, 0, MapMode.Read, MapFlags.None);

            if (image == null)
                image = new Bitmap(Rect.Width, Rect.Height, PixelFormat.Format32bppRgb);

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
            return image?.Clone(new DRectangle(System.Drawing.Point.Empty, image.Size), image.PixelFormat);
        }

        // Capture full
        Bitmap ProcessFrame()
        {
            // Get the desktop capture texture
            var mapSource = _device.ImmediateContext.MapSubresource(_desktopImageTexture, 0, MapMode.Read, MapFlags.None);
            
            if (image == null)
                image = new Bitmap(_outputDesc.DesktopBounds.Width, _outputDesc.DesktopBounds.Height, PixelFormat.Format32bppRgb);
            
            // Copy pixels from screen capture Texture to GDI bitmap
            var mapDest = image.LockBits(new DRectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, image.PixelFormat);

            foreach (var region in MovedRegions)
            {
                var srcPtr = mapDest.Scan0 + region.Source.X * 4 + mapDest.Stride * region.Source.Y;
                var destPtr = mapDest.Scan0 + region.Destination.X * 4 + mapDest.Stride * region.Destination.Y;

                for (int y = 0; y < region.Destination.Height; ++y)
                {
                    Utilities.CopyMemory(destPtr, srcPtr, region.Destination.Width * 4);

                    srcPtr += mapDest.Stride;
                    destPtr += mapDest.Stride;
                }
            }

            foreach (var region in UpdatedRegions)
            {
                var srcPtr = mapSource.DataPointer + region.X * 4 + mapSource.RowPitch * region.Y;
                var destPtr = mapDest.Scan0 + region.X * 4 + mapDest.Stride * region.Y;

                for (int y = 0; y < region.Height; ++y)
                {
                    Utilities.CopyMemory(destPtr, srcPtr, region.Width * 4);

                    srcPtr += mapSource.RowPitch;
                    destPtr += mapDest.Stride;
                }
            }

            // Release source and dest locks
            image.UnlockBits(mapDest);
            _device.ImmediateContext.UnmapSubresource(_desktopImageTexture, 0);
            return image?.Clone(new DRectangle(System.Drawing.Point.Empty, image.Size), image.PixelFormat);
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
