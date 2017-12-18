using System.Drawing;

namespace Screna
{
    public class OneTimeFrame : FrameBase
    {
        public OneTimeFrame(Bitmap Bitmap) : base(Bitmap) { }

        public override void Dispose()
        {
            lock (Bitmap)
            {
                Bitmap.Dispose();
            }
        }

        public override IBitmapEditor GetEditor()
        {
            lock (Bitmap)
                return new OneTimeEditor(Graphics.FromImage(Bitmap));
        }
    }
}