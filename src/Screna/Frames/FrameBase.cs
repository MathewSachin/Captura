using System.Drawing;

namespace Screna
{
    public abstract class FrameBase : IBitmapFrame
    {
        public Bitmap Bitmap { get; }

        protected FrameBase(Bitmap Bitmap)
        {
            this.Bitmap = Bitmap;
        }

        public abstract void Dispose();

        public abstract IBitmapEditor GetEditor();
    }
}