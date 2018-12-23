using System.Drawing;
using Captura;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using Device = SharpDX.Direct3D11.Device;
using Factory = SharpDX.Direct2D1.Factory1;

namespace DesktopDuplication
{
    public class Direct2DEditor : IEditableFrame
    {
        readonly Texture2D _texture;
        readonly Device _device;
        readonly Texture2D _stagingTexture;
        readonly Surface _surface;
        readonly Factory _factory;
        readonly RenderTarget _renderTarget;

        public Direct2DEditor(Texture2D Texture, Device Device, Texture2D StagingTexture)
        {
            _texture = Texture;
            _device = Device;
            _stagingTexture = StagingTexture;
            Width = Texture.Description.Width;
            Height = Texture.Description.Height;

            _surface = Texture.QueryInterface<Surface>();

            _factory = new Factory(FactoryType.SingleThreaded);

            _renderTarget = new RenderTarget(_factory, _surface, new RenderTargetProperties(new PixelFormat(Format.Unknown, AlphaMode.Ignore)));

            _renderTarget.BeginDraw();
        }

        public void Dispose()
        {
            _renderTarget.EndDraw();

            _renderTarget.Dispose();
            _factory.Dispose();
            _surface.Dispose();

            _device.ImmediateContext.CopyResource(_texture, _stagingTexture);
        }

        public float Width { get; }
        public float Height { get; }

        public void DrawImage(object Image, Rectangle? Region, int Opacity = 100)
        {
            
        }

        public void FillRectangle(Color Color, RectangleF Rectangle)
        {
            var rect = new RawRectangleF(Rectangle.Left,
                Rectangle.Top,
                Rectangle.Right,
                Rectangle.Bottom);

            var color = new RawColor4(Color.R / 255f, Color.G / 255f, Color.B / 255f, Color.A / 255f);

            var brush = new SolidColorBrush(_renderTarget, color);

            _renderTarget.FillRectangle(rect, brush);
        }

        public void FillRectangle(Color Color, RectangleF Rectangle, int CornerRadius)
        {
            var rect = new RawRectangleF(Rectangle.Left,
                Rectangle.Top,
                Rectangle.Right,
                Rectangle.Bottom);

            var color = new RawColor4(Color.R / 255f, Color.G / 255f, Color.B / 255f, Color.A / 255f);

            var brush = new SolidColorBrush(_renderTarget, color);

            var roundedRect = new RoundedRectangle
            {
                Rect = rect,
                RadiusX = CornerRadius,
                RadiusY = CornerRadius
            };

            _renderTarget.FillRoundedRectangle(roundedRect, brush);
        }

        public void DrawRectangle(Pen Pen, RectangleF Rectangle)
        {
            
        }

        public void DrawRectangle(Pen Pen, RectangleF Rectangle, int CornerRadius)
        {
            
        }

        public void FillEllipse(Color Color, RectangleF Rectangle)
        {
            var center = new RawVector2(Rectangle.Left + Rectangle.Width / 2f,
                Rectangle.Top + Rectangle.Height / 2f);

            var ellipse = new Ellipse(center,
                Rectangle.Width / 2f,
                Rectangle.Height / 2f);

            var color = new RawColor4(Color.R / 255f, Color.G / 255f, Color.B / 255f, Color.A / 255f);

            var brush = new SolidColorBrush(_renderTarget, color);

            _renderTarget.FillEllipse(ellipse, brush);
        }

        public void DrawEllipse(Pen Pen, RectangleF Rectangle)
        {
            
        }

        public SizeF MeasureString(string Text, Font Font)
        {
            return SizeF.Empty;
        }

        public void DrawString(string Text, Font Font, Color Color, RectangleF LayoutRectangle)
        {
            
        }

        public IBitmapFrame GenerateFrame()
        {
            Dispose();

            return new Texture2DFrame(_stagingTexture, _device);
        }
    }
}