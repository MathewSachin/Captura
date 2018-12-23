using System;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using Device = SharpDX.Direct3D11.Device;
using Factory = SharpDX.DirectWrite.Factory;
using Factory1 = SharpDX.Direct2D1.Factory1;

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

        SolidColorBrush _solidColorBrush;
        Factory _writeFactory;

        public Factory WriteFactory => _writeFactory ?? (_writeFactory = new Factory());

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
            this._texture = Texture;
            this.Device = Device;
            this.StagingTexture = StagingTexture;

            _surface = Texture.QueryInterface<Surface>();

            Factory = new Factory1(FactoryType.SingleThreaded);

            RenderTarget = new RenderTarget(Factory, _surface, new RenderTargetProperties(new PixelFormat(Format.Unknown, AlphaMode.Ignore)));
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
        }
    }
}