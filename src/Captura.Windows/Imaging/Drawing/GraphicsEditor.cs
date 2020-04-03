using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Captura.Windows.Gdi
{
    public class GraphicsEditor : IEditableFrame
    {
        readonly Bitmap _image;
        readonly Graphics _graphics;

        public GraphicsEditor(Bitmap Image)
        {
            _image = Image;

            _graphics = Graphics.FromImage(Image);

            _graphics.SmoothingMode = SmoothingMode.AntiAlias;
        }

        public IBitmapFrame GenerateFrame(TimeSpan Timestamp)
        {
            Dispose();

            return new DrawingFrame(_image, Timestamp);
        }

        public IBitmapImage CreateBitmapBgr32(Size Size, IntPtr MemoryData, int Stride)
        {
            return GraphicsBitmapLoader.Instance.CreateBitmapBgr32(Size, MemoryData, Stride);
        }

        public IBitmapImage LoadBitmap(string FileName)
        {
            return GraphicsBitmapLoader.Instance.LoadBitmap(FileName);
        }

        public void DrawLine(Point Start, Point End, Color Color, float Width)
        {
            _graphics.DrawLine(new Pen(new SolidBrush(Color), Width), Start, End);
        }

        public void DrawArrow(Point Start, Point End, Color Color, float Width)
        {
            using var pen = new Pen(new SolidBrush(Color), Width)
            {
                EndCap = LineCap.ArrowAnchor
            };

            _graphics.DrawLine(pen, Start, End);
        }

        public void DrawImage(IBitmapImage Image, RectangleF? Region, int Opacity = 100)
        {
            if (!(Image is DrawingImage drawingImage))
                return;

            var img = drawingImage.Image;

            var region = Region is RectangleF r
                ? new Rectangle((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height)
                : new Rectangle(Point.Empty, img.Size);

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

        public IFont GetFont(string FontFamily, int Size)
        {
            return new DrawingFont(FontFamily, Size);
        }

        public SizeF MeasureString(string Text, IFont Font)
        {
            if (Font is DrawingFont font)
            {
                return _graphics.MeasureString(Text, font.Font, PointF.Empty, StringFormat.GenericTypographic);
            }

            return SizeF.Empty;
        }

        public void DrawString(string Text, IFont Font, Color Color, RectangleF LayoutRectangle)
        {
            if (Font is DrawingFont font)
            {
                _graphics.DrawString(Text, font.Font, new SolidBrush(Color), LayoutRectangle, StringFormat.GenericTypographic);
            }
        }

        public float Width => _graphics.VisibleClipBounds.Width;

        public float Height => _graphics.VisibleClipBounds.Height;

        public void Dispose()
        {
            _graphics.Dispose();
        }
    }
}