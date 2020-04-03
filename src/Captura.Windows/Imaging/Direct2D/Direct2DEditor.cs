using System;
using System.Drawing;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using SharpDX.WIC;
using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using Bitmap = SharpDX.Direct2D1.Bitmap;
using BitmapInterpolationMode = SharpDX.Direct2D1.BitmapInterpolationMode;
using Color = System.Drawing.Color;
using PixelFormat = SharpDX.WIC.PixelFormat;
using Point = System.Drawing.Point;
using RectangleF = System.Drawing.RectangleF;

namespace Captura.Windows.DirectX
{
    public class Direct2DEditor : IEditableFrame
    {
        readonly Direct2DEditorSession _editorSession;

        public Direct2DEditor(Direct2DEditorSession EditorSession)
        {
            _editorSession = EditorSession;

            var desc = EditorSession.StagingTexture.Description;

            Width = desc.Width;
            Height = desc.Height;

            EditorSession.BeginDraw();
        }

        public void Dispose() { }

        public float Width { get; }
        public float Height { get; }

        public IBitmapImage CreateBitmapBgr32(Size Size, IntPtr MemoryData, int Stride)
        {
            var bmp = new Bitmap(_editorSession.RenderTarget,
                new Size2(Size.Width, Size.Height),
                new DataPointer(MemoryData, Size.Height * Stride),
                Stride,
                new BitmapProperties(new SharpDX.Direct2D1.PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Ignore)));

            return new Direct2DImage(bmp);
        }

        public IBitmapImage LoadBitmap(string FileName)
        {
            using var decoder = new BitmapDecoder(_editorSession.ImagingFactory, FileName, 0);
            using var bmpSource = decoder.GetFrame(0);
            using var convertedBmp = new FormatConverter(_editorSession.ImagingFactory);
            convertedBmp.Initialize(bmpSource, PixelFormat.Format32bppPBGRA);

            var bmp = Bitmap.FromWicBitmap(_editorSession.RenderTarget, convertedBmp);

            return new Direct2DImage(bmp);
        }

        public void DrawImage(IBitmapImage Image, RectangleF? Region, int Opacity = 100)
        {
            if (Image is Direct2DImage direct2DImage)
            {
                var bmp = direct2DImage.Bitmap;

                var rect = Region ?? new RectangleF(0, 0, bmp.Size.Width, bmp.Size.Height);
                var rawRect = new RawRectangleF(rect.Left,
                    rect.Top,
                    rect.Right,
                    rect.Bottom);

                _editorSession.RenderTarget.DrawBitmap(bmp, rawRect, Opacity, BitmapInterpolationMode.Linear);
            }
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

        RawVector2 Convert(Point P)
        {
            return new RawVector2(P.X, P.Y);
        }

        Ellipse ToEllipse(RectangleF Rectangle)
        {
            var center = new RawVector2(Rectangle.Left + Rectangle.Width / 2f,
                Rectangle.Top + Rectangle.Height / 2f);

            return new Ellipse(center,
                Rectangle.Width / 2f,
                Rectangle.Height / 2f);
        }

        public void DrawLine(Point Start, Point End, Color Color, float Width)
        {
            _editorSession.RenderTarget.DrawLine(Convert(Start), Convert(End), Convert(Color), Width);
        }

        public void DrawArrow(Point Start, Point End, Color Color, float Width)
        {
            var props = new StrokeStyleProperties
            {
                EndCap = CapStyle.Round,
                LineJoin = LineJoin.Round
            };

            var style = new StrokeStyle(_editorSession.RenderTarget.Factory, props);

            _editorSession.RenderTarget.DrawLine(Convert(Start), Convert(End), Convert(Color), Width, style);

            var direction = new Vector2(End.X - Start.X, End.Y - Start.Y);
            var theta = Math.Atan2(direction.Y, direction.X);

            const double rotateBy = 3 * Math.PI / 4;
            var sideLen = Width * 2;

            // Start drawing from ending point
            Start = End;

            var ltheta = theta - rotateBy;
            End = new Point((int)(Start.X + sideLen * Math.Cos(ltheta)), (int)(Start.Y + sideLen * Math.Sin(ltheta)));
            _editorSession.RenderTarget.DrawLine(Convert(Start), Convert(End), Convert(Color), Width, style);

            var rtheta = theta + rotateBy;
            End = new Point((int)(Start.X + sideLen * Math.Cos(rtheta)), (int)(Start.Y + sideLen * Math.Sin(rtheta)));
            _editorSession.RenderTarget.DrawLine(Convert(Start), Convert(End), Convert(Color), Width, style);
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

        public IFont GetFont(string FontFamily, int Size)
        {
            return new Direct2DFont(FontFamily, Size, _editorSession.WriteFactory);
        }

        TextLayout GetTextLayout(string Text, TextFormat Format)
        {
            return new TextLayout(_editorSession.WriteFactory, Text, Format, Width, Height);
        }

        public SizeF MeasureString(string Text, IFont Font)
        {
            if (Font is Direct2DFont font)
            {
                using var layout = GetTextLayout(Text, font.TextFormat);
                return new SizeF(layout.Metrics.Width, layout.Metrics.Height);
            }

            return SizeF.Empty;
        }

        public void DrawString(string Text, IFont Font, Color Color, RectangleF LayoutRectangle)
        {
            if (Font is Direct2DFont font)
            {
                using var layout = GetTextLayout(Text, font.TextFormat);
                _editorSession.RenderTarget.DrawTextLayout(
                    new RawVector2(LayoutRectangle.X, LayoutRectangle.Y),
                    layout,
                    Convert(Color));
            }
        }

        public IBitmapFrame GenerateFrame(TimeSpan Timestamp)
        {
            _editorSession.EndDraw();

            return new Texture2DFrame(_editorSession.StagingTexture,
                _editorSession.Device,
                _editorSession.PreviewTexture,
                Timestamp,
                _editorSession.ColorConverter);
        }
    }
}