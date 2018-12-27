using System;
using System.Drawing;

namespace Captura
{
    public class RepeatFrame : IBitmapFrame, IEditableFrame
    {
        RepeatFrame() { }

        public static RepeatFrame Instance { get; } = new RepeatFrame();

        IBitmapFrame IEditableFrame.GenerateFrame() => Instance;

        int IBitmapFrame.Width { get; } = -1;

        float IEditableFrame.Height { get; } = -1;

        IDisposable IBitmapLoader.CreateBitmapBgr32(Size Size, IntPtr MemoryData, int Stride)
        {
            throw new NotImplementedException();
        }

        IDisposable IBitmapLoader.LoadBitmap(string FileName, out Size Size)
        {
            throw new NotImplementedException();
        }

        float IEditableFrame.Width { get; } = -1;

        int IBitmapFrame.Height { get; } = -1;

        void IDisposable.Dispose() { }

        void IEditableFrame.DrawImage(object Image, Rectangle? Region, int Opacity)
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

        SizeF IEditableFrame.MeasureString(string Text, int FontSize)
        {
            throw new NotImplementedException();
        }

        void IEditableFrame.DrawString(string Text, int FontSize, Color Color, RectangleF LayoutRectangle)
        {
            throw new NotImplementedException();
        }

        void IBitmapFrame.CopyTo(byte[] Buffer, int Length)
        {
            throw new NotImplementedException();
        }
    }
}