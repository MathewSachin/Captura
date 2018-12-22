using System;
using System.Drawing;
using System.IO;

namespace Captura
{
    public class RepeatFrame : IBitmapFrame, IEditableFrame
    {
        RepeatFrame() { }

        public static RepeatFrame Instance { get; } = new RepeatFrame();

        IBitmapFrame IEditableFrame.GenerateFrame() => RepeatFrame.Instance;

        int IBitmapFrame.Width { get; } = -1;

        float IEditableFrame.Height { get; } = -1;

        float IEditableFrame.Width { get; } = -1;

        int IBitmapFrame.Height { get; } = -1;

        void IDisposable.Dispose() { }

        void IBitmapFrame.SaveGif(Stream Stream)
        {
            throw new NotImplementedException();
        }

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

        void IEditableFrame.DrawRectangle(Pen Pen, RectangleF Rectangle)
        {
            throw new NotImplementedException();
        }

        void IEditableFrame.DrawRectangle(Pen Pen, RectangleF Rectangle, int CornerRadius)
        {
            throw new NotImplementedException();
        }

        void IEditableFrame.FillEllipse(Color Color, RectangleF Rectangle)
        {
            throw new NotImplementedException();
        }

        void IEditableFrame.DrawEllipse(Pen Pen, RectangleF Rectangle)
        {
            throw new NotImplementedException();
        }

        SizeF IEditableFrame.MeasureString(string Text, Font Font)
        {
            throw new NotImplementedException();
        }

        void IEditableFrame.DrawString(string Text, Font Font, Color Color, RectangleF LayoutRectangle)
        {
            throw new NotImplementedException();
        }

        void IBitmapFrame.CopyTo(byte[] Buffer, int Length)
        {
            throw new NotImplementedException();
        }
    }
}