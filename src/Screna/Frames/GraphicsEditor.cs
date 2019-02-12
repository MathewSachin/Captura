using System;
using System.Drawing;
using System.Drawing.Imaging;
using Captura;
using Captura.Models;

namespace Screna
{
    public class GraphicsEditor : IEditableFrame
    {
        readonly Bitmap _image;
        readonly Graphics _graphics;

        public GraphicsEditor(Bitmap Image)
        {
            _image = Image;

            _graphics = Graphics.FromImage(Image);
        }

        public IBitmapFrame GenerateFrame()
        {
            Dispose();

            return new OneTimeFrame(_image);
        }

        public IBitmapImage CreateBitmapBgr32(Size Size, IntPtr MemoryData, int Stride)
        {
            return GraphicsBitmapLoader.Instance.CreateBitmapBgr32(Size, MemoryData, Stride);
        }

        public IBitmapImage LoadBitmap(string FileName)
        {
            return GraphicsBitmapLoader.Instance.LoadBitmap(FileName);
        }

        public void DrawImage(IBitmapImage Image, Rectangle? Region, int Opacity = 100)
        {
            if (!(Image is DrawingImage drawingImage))
                return;

            var img = drawingImage.Image;

            var region = Region ?? new Rectangle(Point.Empty, img.Size);

            if (Opacity < 100)
            {
                var colormatrix = new ColorMatrix
                {
                    Matrix33 = Opacity / 100.0f
                };

                var imgAttribute = new ImageAttributes();
                imgAttribute.SetColorMatrix(colormatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                _graphics.DrawImage(img, region, 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imgAttribute);
            }
            else _graphics.DrawImage(img, region);
        }

        public void FillRectangle(Color Color, RectangleF Rectangle)
        {
            _graphics.FillRectangle(new SolidBrush(Color), Rectangle);
        }

        public void FillRectangle(Color Color, RectangleF Rectangle, int CornerRadius)
        {
            _graphics.FillRoundedRectangle(new SolidBrush(Color), Rectangle, CornerRadius);
        }

        public void FillEllipse(Color Color, RectangleF Rectangle)
        {
            _graphics.FillEllipse(new SolidBrush(Color), Rectangle);
        }

        public void DrawEllipse(Color Color, float StrokeWidth, RectangleF Rectangle)
        {
            _graphics.DrawEllipse(new Pen(Color, StrokeWidth), Rectangle);
        }

        public void DrawRectangle(Color Color, float StrokeWidth, RectangleF Rectangle)
        {
            _graphics.DrawRectangle(new Pen(Color, StrokeWidth), Rectangle.X, Rectangle.Y, Rectangle.Width, Rectangle.Height);
        }

        public void DrawRectangle(Color Color, float StrokeWidth, RectangleF Rectangle, int CornerRadius)
        {
            _graphics.DrawRoundedRectangle(new Pen(Color, StrokeWidth), Rectangle, CornerRadius);
        }

        public SizeF MeasureString(string Text, int FontSize)
        {
            var font = new Font(FontFamily.GenericMonospace, FontSize);

            return _graphics.MeasureString(Text, font);
        }

        public void DrawString(string Text, int FontSize, Color Color, RectangleF LayoutRectangle)
        {
            var font = new Font(FontFamily.GenericMonospace, FontSize);

            _graphics.DrawString(Text, font, new SolidBrush(Color), LayoutRectangle);
        }

        public float Width => _graphics.VisibleClipBounds.Width;

        public float Height => _graphics.VisibleClipBounds.Height;

        public void Dispose()
        {
            _graphics.Dispose();
        }
    }
}