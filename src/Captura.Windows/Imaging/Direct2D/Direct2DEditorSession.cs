using System;
using Captura.Models;
using SharpDX.Direct2D1;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using SharpDX.WIC;
using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using Device = SharpDX.Direct3D11.Device;
using Factory = SharpDX.DirectWrite.Factory;
using Factory1 = SharpDX.Direct2D1.Factory1;
using FeatureLevel = SharpDX.Direct3D.FeatureLevel;
using PixelFormat = SharpDX.Direct2D1.PixelFormat;

namespace DesktopDuplication
{
    public class Direct2DEditorSession : IDisposable
    {
        readonly IPreviewWindow _previewWindow;
        public Texture2D DesktopTexture { get; private set; }

        public Device Device { get; private set; }
        public Texture2D StagingTexture { get; private set; }
        public RenderTarget RenderTarget { get; private set; }
        public Texture2D PreviewTexture { get; private set; }

        SolidColorBrush _solidColorBrush;
        Factory1 _factory;
        Factory _writeFactory;
        ImagingFactory _imagingFactory;

        public Factory WriteFactory => _writeFactory ?? (_writeFactory = new Factory());

        public ImagingFactory ImagingFactory => _imagingFactory ?? (_imagingFactory = new ImagingFactory());

        public SolidColorBrush GetSolidColorBrush(RawColor4 Color)
        {
            if (_solidColorBrush == null)
            {
                _solidColorBrush = new SolidColorBrush(RenderTarget, Color);
            }
            else _solidColorBrush.Color = Color;

            return _solidColorBrush;
        }

        public Direct2DEditorSession(int Width, int Height, IPreviewWindow PreviewWindow)
        {
            _previewWindow = PreviewWindow;

            Device = new Device(DriverType.Hardware,
                DeviceCreationFlags.BgraSupport,
                FeatureLevel.Level_11_1);

            StagingTexture = new Texture2D(Device, new Texture2DDescription
            {
                CpuAccessFlags = CpuAccessFlags.Read,
                BindFlags = BindFlags.None,
                Format = Format.B8G8R8A8_UNorm,
                Width = Width,
                Height = Height,
                OptionFlags = ResourceOptionFlags.None,
                MipLevels = 1,
                ArraySize = 1,
                SampleDescription = { Count = 1, Quality = 0 },
                Usage = ResourceUsage.Staging
            });

            DesktopTexture = new Texture2D(Device, new Texture2DDescription
            {
                CpuAccessFlags = CpuAccessFlags.None,
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                Format = Format.B8G8R8A8_UNorm,
                Width = Width,
                Height = Height,
                OptionFlags = ResourceOptionFlags.None,
                MipLevels = 1,
                ArraySize = 1,
                SampleDescription = { Count = 1, Quality = 0 },
                Usage = ResourceUsage.Default
            });

            var desc = DesktopTexture.Description;
            desc.OptionFlags = ResourceOptionFlags.Shared;

            PreviewTexture = new Texture2D(Device, desc);

            _factory = new Factory1(FactoryType.MultiThreaded);

            var pixelFormat = new PixelFormat(Format.Unknown, AlphaMode.Ignore);

            var renderTargetProps = new RenderTargetProperties(pixelFormat)
            {
                Type = RenderTargetType.Hardware
            };

            using (var surface = DesktopTexture.QueryInterface<Surface>())
            {
                RenderTarget = new RenderTarget(_factory, surface, renderTargetProps);
            }
        }

        public void BeginDraw()
        {
            RenderTarget.BeginDraw();
        }

        public void EndDraw()
        {
            RenderTarget.EndDraw();
            Device.ImmediateContext.CopyResource(DesktopTexture, StagingTexture);

            if (_previewWindow.IsVisible)
            {
                Device.ImmediateContext.CopyResource(StagingTexture, PreviewTexture);
            }

            // Actual CopyResource happens here
            Device.ImmediateContext.Flush();
        }

        public void Dispose()
        {
            _solidColorBrush?.Dispose();
            _solidColorBrush = null;

            RenderTarget.Dispose();
            RenderTarget = null;

            _factory.Dispose();
            _factory = null;

            _writeFactory?.Dispose();
            _writeFactory = null;

            _imagingFactory?.Dispose();
            _imagingFactory = null;

            PreviewTexture.Dispose();
            PreviewTexture = null;

            DesktopTexture.Dispose();
            DesktopTexture = null;

            StagingTexture.Dispose();
            StagingTexture = null;

            Device.Dispose();
            Device = null;
        }
    }
}