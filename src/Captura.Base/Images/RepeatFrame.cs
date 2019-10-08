using System;
using System.Drawing;

namespace Captura
{
    public class RepeatFrame : IBitmapFrame, IEditableFrame
    {
        RepeatFrame() { }

        public static RepeatFrame Instance { get; } = new RepeatFrame();

        IBitmapFrame IEditableFrame.GenerateFrame(TimeSpan Timestamp) => Instance;

        int IBitmapFrame.Width { get; } = -1;

        float IEditableFrame.Height { get; } = -1;

        TimeSpan IBitmapFrame.Timestamp { get; }

        IBitmapImage IBitmapLoader.CreateBitmapBgr32(Size Size, IntPtr MemoryData, int Stride)
        {
            throw new NotImplementedException();
        }

        IBitmapImage IBitmapLoader.LoadBitmap(string FileName)
        {
            throw new NotImplementedException();
        }

        float IEditableFrame.Width { get; } = -1;

        int IBitmapFrame.Height { get; } = -1;

        void IDisposable.Dispose() { }

        void IEditableFrame.DrawLine(Point Start, Point End, Color Color, float Width)
        {
            throw new NotImplementedException();
        }

        void IEditableFrame.DrawArrow(Point Start, Point End, Color Color, float Width)
        {
            throw new NotImplementedException();
        }

        void IEditableFrame.DrawImage(IBitmapImage Image, RectangleF? Region, int Opacity)
        {
            throw new NotImplementedException();
        }

        void IEditableFrame.FillRectangle(Color Color, RectangleF Rectangle)
        {
            throw new NotImplementedException();
        }

        void IEditableFrame.FillRectangle(Color Color, RectangleF Rectangle, int CornerRadius)
        {
            throw new NotImplementedException();
        }

        void IEditableFrame.DrawRectangle(Color Color, float StrokeWidth, RectangleF Rectangle)
        {
            throw new NotImplementedException();
        }

        void IEditableFrame.DrawRectangle(Color Color, float StrokeWidth, RectangleF Rectangle, int CornerRadius)
        {
            throw new NotImplementedException();
        }

        void IEditableFrame.FillEllipse(Color Color, RectangleF Rectangle)
        {
            throw new NotImplementedException();
        }

        void IEditableFrame.DrawEllipse(Color Color, float StrokeWidth, RectangleF Rectangle)
        {
            throw new NotImplementedException();
        }

        IFont IEditableFrame.GetFont(string FontFamily, int Size)
        {
            throw new NotImplementedException();
        }

        SizeF IEditableFrame.MeasureString(string Text, IFont Font)
        {
            throw new NotImplementedException();
        }

        void IEditableFrame.DrawString(string Text, IFont Font, Color Color, RectangleF LayoutRectangle)
        {
            throw new NotImplementedException();
        }

        void IBitmapFrame.CopyTo(byte[] Buffer)
        {
            throw new NotImplementedException();
        }

        void IBitmapFrame.CopyTo(IntPtr Buffer)
        {
            throw new NotImplementedException();
        }
    }
}