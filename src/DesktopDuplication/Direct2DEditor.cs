using System.Drawing;
using Captura;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX.Mathematics.Interop;

namespace DesktopDuplication
{
    public class Direct2DEditor : IEditableFrame
    {
        readonly Direct2DEditorSession _editorSession;

        public Direct2DEditor(Direct2DEditorSession EditorSession)
        {
            _editorSession = EditorSession;

            Width = EditorSession.StagingTexture.Description.Width;
            Height = EditorSession.StagingTexture.Description.Height;

            EditorSession.BeginDraw();
        }

        public void Dispose()
        {
            _editorSession.EndDraw();
        }

        public float Width { get; }
        public float Height { get; }

        public void DrawImage(object Image, Rectangle? Region, int Opacity = 100)
        {
            
        }

        SolidColorBrush Convert(Color Color)
        {
            var color = new RawColor4(Color.R / 255f, Color.G / 255f, Color.B / 255f, Color.A / 255f);

            return _editorSession.GetSolidColorBrush(color);
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
            _editorSession.RenderTarget.FillRectangle(Convert(Rectangle), Convert(Color));
        }

        public void FillRectangle(Color Color, RectangleF Rectangle, int CornerRadius)
        {
            _editorSession.RenderTarget.FillRoundedRectangle(Convert(Rectangle, CornerRadius), Convert(Color));
        }

        public void DrawRectangle(Color Color, float StrokeWidth, RectangleF Rectangle)
        {
            _editorSession.RenderTarget.DrawRectangle(Convert(Rectangle), Convert(Color), StrokeWidth);
        }

        public void DrawRectangle(Color Color, float StrokeWidth, RectangleF Rectangle, int CornerRadius)
        {
            _editorSession.RenderTarget.DrawRoundedRectangle(Convert(Rectangle, CornerRadius), Convert(Color), StrokeWidth);
        }

        public void FillEllipse(Color Color, RectangleF Rectangle)
        {
            _editorSession.RenderTarget.FillEllipse(ToEllipse(Rectangle), Convert(Color));
        }

        public void DrawEllipse(Color Color, float StrokeWidth, RectangleF Rectangle)
        {
            _editorSession.RenderTarget.DrawEllipse(ToEllipse(Rectangle), Convert(Color), StrokeWidth);
        }

        TextFormat GetTextFormat(int FontSize)
        {
            return new TextFormat(_editorSession.WriteFactory, "Arial", FontSize);
        }

        TextLayout GetTextLayout(string Text, TextFormat Format)
        {
            return new TextLayout(_editorSession.WriteFactory, Text, Format, Width, Height);
        }

        public SizeF MeasureString(string Text, int FontSize)
        {
            using (var format = GetTextFormat(FontSize))
            using (var layout = GetTextLayout(Text, format))
            {
                return new SizeF(layout.Metrics.Width, layout.Metrics.Height);
            }
        }

        public void DrawString(string Text, int FontSize, Color Color, RectangleF LayoutRectangle)
        {
            using (var format = GetTextFormat(FontSize))
            using (var layout = GetTextLayout(Text, format))
            {
                _editorSession.RenderTarget.DrawTextLayout(
                    new RawVector2(LayoutRectangle.X, LayoutRectangle.Y),
                    layout,
                    Convert(Color));
            }
        }

        public IBitmapFrame GenerateFrame()
        {
            Dispose();

            return new Texture2DFrame(_editorSession.StagingTexture, _editorSession.Device);
        }
    }
}