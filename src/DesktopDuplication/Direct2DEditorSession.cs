using System;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using SharpDX.WIC;
using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using Device = SharpDX.Direct3D11.Device;
using Factory = SharpDX.DirectWrite.Factory;
using Factory1 = SharpDX.Direct2D1.Factory1;
using PixelFormat = SharpDX.Direct2D1.PixelFormat;

namespace DesktopDuplication
{
    public class Direct2DEditorSession : IDisposable
    {
        readonly Surface _surface;
        readonly Texture2D _texture;

        public Device Device { get; }
        public Texture2D StagingTexture { get; }
        public Factory1 Factory { get; }
        public RenderTarget RenderTarget { get; }
        public Texture2D PreviewTexture { get; }

        SolidColorBrush _solidColorBrush;
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

        public Direct2DEditorSession(Texture2D Texture, Device Device, Texture2D StagingTexture)
        {
            _texture = Texture;
            this.Device = Device;
            this.StagingTexture = StagingTexture;

            PreviewTexture = new Texture2D(Device, new Texture2DDescription
            {
                CpuAccessFlags = CpuAccessFlags.None,
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                Format = Format.B8G8R8A8_UNorm,
                Width = Texture.Description.Width,
                Height = Texture.Description.Height,
                OptionFlags = ResourceOptionFlags.Shared,
                MipLevels = 1,
                ArraySize = 1,
                SampleDescription = { Count = 1, Quality = 0 },
                Usage = ResourceUsage.Default
            });

            _surface = Texture.QueryInterface<Surface>();

            Factory = new Factory1(FactoryType.MultiThreaded);

            var renderTargetProps = new RenderTargetProperties(
                new PixelFormat(Format.Unknown, AlphaMode.Ignore))
            {
                Type = RenderTargetType.Hardware
            };

            RenderTarget = new RenderTarget(Factory, _surface, renderTargetProps);
        }

        public void BeginDraw()
        {
            RenderTarget.BeginDraw();
        }

        public void EndDraw()
        {
            RenderTarget.EndDraw();
            Device.ImmediateContext.CopyResource(_texture, StagingTexture);
        }

        public void Dispose()
        {
            _solidColorBrush?.Dispose();

            RenderTarget.Dispose();
            Factory.Dispose();
            _surface.Dispose();

            _writeFactory?.Dispose();
            _imagingFactory?.Dispose();

            PreviewTexture.Dispose();
        }
    }
}