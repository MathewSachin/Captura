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
        readonly Device mDevice;
        readonly Texture2DDescription mTextureDesc;
        OutputDescription mOutputDesc;
        readonly OutputDuplication mDeskDupl;

        Texture2D desktopImageTexture;
        OutputDuplicateFrameInformation frameInfo;
        
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

            mDevice = new Device(adapter);

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
            mOutputDesc = output.Description;
            
            mTextureDesc = new Texture2DDescription
            {
                CpuAccessFlags = CpuAccessFlags.Read,
                BindFlags = BindFlags.None,
                Format = Format.B8G8R8A8_UNorm,
                Width = mOutputDesc.DesktopBounds.Width,
                Height = mOutputDesc.DesktopBounds.Height,
                OptionFlags = ResourceOptionFlags.None,
                MipLevels = 1,
                ArraySize = 1,
                SampleDescription = { Count = 1, Quality = 0 },
                Usage = ResourceUsage.Staging
            };

            try
            {
                mDeskDupl = output1.DuplicateOutput(mDevice);
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
                RetrieveFrameMetadata();

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
            if (desktopImageTexture == null)
                desktopImageTexture = new Texture2D(mDevice, mTextureDesc);

            SharpDX.DXGI.Resource desktopResource = null;

            frameInfo = new OutputDuplicateFrameInformation();

            try
            {
                mDeskDupl.AcquireNextFrame(500, out frameInfo, out desktopResource);
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
                mDevice.ImmediateContext.CopyResource(tempTexture, desktopImageTexture);

            desktopResource.Dispose();

            return true;
        }

        MovedRegion[] MovedRegions;
        DRectangle[] UpdatedRegions;

        void RetrieveFrameMetadata()
        {
            if (frameInfo.TotalMetadataBufferSize > 0)
            {
                // Get moved regions
                var movedRectangles = new OutputDuplicateMoveRectangle[frameInfo.TotalMetadataBufferSize];
                mDeskDupl.GetFrameMoveRects(movedRectangles.Length, movedRectangles, out int movedRegionsLength);

                MovedRegions = new MovedRegion[movedRegionsLength / Marshal.SizeOf<OutputDuplicateMoveRectangle>()];

                for (int i = 0; i < MovedRegions.Length; i++)
                {
                    MovedRegions[i] = new MovedRegion
                    {
                        Source = new System.Drawing.Point(movedRectangles[i].SourcePoint.X, movedRectangles[i].SourcePoint.Y),
                        Destination = new DRectangle(movedRectangles[i].DestinationRect.X, movedRectangles[i].DestinationRect.Y, movedRectangles[i].DestinationRect.Width, movedRectangles[i].DestinationRect.Height)
                    };
                }

                // Get dirty regions
                var dirtyRectangles = new SharpDX.Rectangle[frameInfo.TotalMetadataBufferSize];
                mDeskDupl.GetFrameDirtyRects(dirtyRectangles.Length, dirtyRectangles, out int dirtyRegionsLength);

                UpdatedRegions = new DRectangle[dirtyRegionsLength / Marshal.SizeOf<SharpDX.Rectangle>()];

                for (int i = 0; i < UpdatedRegions.Length; i++)
                {
                    UpdatedRegions[i] = new DRectangle(dirtyRectangles[i].X, dirtyRectangles[i].Y, dirtyRectangles[i].Width, dirtyRectangles[i].Height);
                }
            }
            else
            {
                MovedRegions = new MovedRegion[0];
                UpdatedRegions = new DRectangle[0];
            }
        }

        Bitmap image;

        Bitmap ProcessFrame(DRectangle Rect)
        {
            // Get the desktop capture texture
            var mapSource = mDevice.ImmediateContext.MapSubresource(desktopImageTexture, 0, MapMode.Read, MapFlags.None);
            
            if (image == null)
                image = new Bitmap(Rect.Width, Rect.Height, PixelFormat.Format32bppRgb);
            
            // Copy pixels from screen capture Texture to GDI bitmap
            var mapDest = image.LockBits(new DRectangle(System.Drawing.Point.Empty, Rect.Size), ImageLockMode.ReadWrite, image.PixelFormat);

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
            mDevice.ImmediateContext.UnmapSubresource(desktopImageTexture, 0);
            return image?.Clone(new DRectangle(System.Drawing.Point.Empty, image.Size), image.PixelFormat);
        }

        void ReleaseFrame()
        {
            try
            {
                mDeskDupl.ReleaseFrame();
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
            mDeskDupl?.Dispose();
            desktopImageTexture?.Dispose();
            mDevice?.Dispose();
        }
    }
}
