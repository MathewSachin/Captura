﻿// Adapted from https://github.com/jasonpang/desktop-duplication-net

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
        readonly Texture2D _desktopImageTexture;
        readonly Texture2D _stagingTexture;
        readonly Rectangle _rect;
        readonly bool _includeCursor;
        readonly DuplCapture _duplCapture;
        readonly Device _device;

        public int Timeout { get; set; }

        public DesktopDuplicator(Rectangle Rect, bool IncludeCursor, Adapter Adapter, Output1 Output)
        {
            _device = new Device(Adapter, DeviceCreationFlags.BgraSupport);

            _duplCapture = new DuplCapture(_device, Output);
            _rect = Rect;
            _includeCursor = IncludeCursor;

            _stagingTexture = new Texture2D(_device, new Texture2DDescription
            {
                CpuAccessFlags = CpuAccessFlags.Read,
                BindFlags = BindFlags.None,
                Format = Format.B8G8R8A8_UNorm,
                Width = Rect.Width,
                Height = Rect.Height,
                OptionFlags = ResourceOptionFlags.None,
                MipLevels = 1,
                ArraySize = 1,
                SampleDescription = { Count = 1, Quality = 0 },
                Usage = ResourceUsage.Staging
            });

            _desktopImageTexture = new Texture2D(_device, new Texture2DDescription
            {
                CpuAccessFlags = CpuAccessFlags.None,
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                Format = Format.B8G8R8A8_UNorm,
                Width = Rect.Width,
                Height = Rect.Height,
                OptionFlags = ResourceOptionFlags.None,
                MipLevels = 1,
                ArraySize = 1,
                SampleDescription = { Count = 1, Quality = 0 },
                Usage = ResourceUsage.Default
            });
        }
        
        public IEditableFrame Capture()
        {
            OutputDuplicateFrameInformation? frameInfo;

            try
            {
                frameInfo = _duplCapture.Get(_desktopImageTexture, Timeout);
            }
            catch
            {
                try { _duplCapture.Init(); }
                catch
                {
                    // ignored
                }

                return RepeatFrame.Instance;
            }

            if (frameInfo is null)
                return RepeatFrame.Instance;

            return new Direct2DEditor(_desktopImageTexture, _device, _stagingTexture);

            var mapSource = _device.ImmediateContext.MapSubresource(_desktopImageTexture, 0, MapMode.Read, MapFlags.None);

            try
            {
                var bmp = ProcessFrame(mapSource.DataPointer, mapSource.RowPitch, frameInfo.Value);

                var editor = new GraphicsEditor(bmp);

                if (_includeCursor && (frameInfo.Value.LastMouseUpdateTime == 0 || frameInfo.Value.PointerPosition.Visible))
                {
                    MouseCursor.Draw(editor, P => new Point(P.X - _rect.X, P.Y - _rect.Y));
                }

                return editor;
            }
            finally
            {
                _device.ImmediateContext.UnmapSubresource(_desktopImageTexture, 0);
            }
        }

        Bitmap ProcessFrame(IntPtr SourcePtr, int SourceRowPitch, OutputDuplicateFrameInformation FrameInfo)
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

            return frame;
        }

        public void Dispose()
        {
            try
            {
                _duplCapture?.Dispose();
                _desktopImageTexture?.Dispose();
                _device?.Dispose();
            }
            catch { }
        }
    }
}
