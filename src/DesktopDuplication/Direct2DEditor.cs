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

        SolidColorBrush Convert(Color Color)
        {
            var color = new RawColor4(Color.R / 255f, Color.G / 255f, Color.B / 255f, Color.A / 255f);

            return new SolidColorBrush(_renderTarget, color);
        }

        RawRectangleF Convert(RectangleF Rectangle)
        {
            return new RawRectangleF(Rectangle.Left,
                Rectangle.Top,
                Rectangle.Right,
                Rectangle.Bottom);
        }

        RoundedRectangle Convert(RectangleF Rectangle, int CornerRadius)
        {
            return new RoundedRectangle
            {
                Rect = Convert(Rectangle),
                RadiusX = CornerRadius,
                RadiusY = CornerRadius
            };
        }

        Ellipse ToEllipse(RectangleF Rectangle)
        {
            var center = new RawVector2(Rectangle.Left + Rectangle.Width / 2f,
                Rectangle.Top + Rectangle.Height / 2f);

            return new Ellipse(center,
                Rectangle.Width / 2f,
                Rectangle.Height / 2f);
        }

        public void FillRectangle(Color Color, RectangleF Rectangle)
        {
            _renderTarget.FillRectangle(Convert(Rectangle), Convert(Color));
        }

        public void FillRectangle(Color Color, RectangleF Rectangle, int CornerRadius)
        {
            _renderTarget.FillRoundedRectangle(Convert(Rectangle, CornerRadius), Convert(Color));
        }

        public void DrawRectangle(Color Color, float StrokeWidth, RectangleF Rectangle)
        {
            _renderTarget.DrawRectangle(Convert(Rectangle), Convert(Color), StrokeWidth);
        }

        public void DrawRectangle(Color Color, float StrokeWidth, RectangleF Rectangle, int CornerRadius)
        {
            _renderTarget.DrawRoundedRectangle(Convert(Rectangle, CornerRadius), Convert(Color), StrokeWidth);
        }

        public void FillEllipse(Color Color, RectangleF Rectangle)
        {
            _renderTarget.FillEllipse(ToEllipse(Rectangle), Convert(Color));
        }

        public void DrawEllipse(Color Color, float StrokeWidth, RectangleF Rectangle)
        {
            _renderTarget.DrawEllipse(ToEllipse(Rectangle), Convert(Color), StrokeWidth);
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