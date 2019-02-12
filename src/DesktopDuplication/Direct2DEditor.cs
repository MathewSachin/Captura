using System;
using System.Drawing;
using Captura;
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
using Rectangle = System.Drawing.Rectangle;
using RectangleF = System.Drawing.RectangleF;

namespace DesktopDuplication
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
            using (var decoder = new BitmapDecoder(_editorSession.ImagingFactory, FileName, 0))
            using (var bmpSource = decoder.GetFrame(0))
            using (var convertedBmp = new FormatConverter(_editorSession.ImagingFactory))
            {
                convertedBmp.Initialize(bmpSource, PixelFormat.Format32bppPBGRA);

                var bmp = Bitmap.FromWicBitmap(_editorSession.RenderTarget, convertedBmp);

                return new Direct2DImage(bmp);
            }
        }

        public void DrawImage(IBitmapImage Image, Rectangle? Region, int Opacity = 100)
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
            _editorSession.EndDraw();

            return new Texture2DFrame(_editorSession.StagingTexture, _editorSession.Device, _editorSession.PreviewTexture);
        }
    }
}