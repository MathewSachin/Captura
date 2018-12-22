using System.Drawing;

namespace Screna
{
    public class OneTimeFrame : FrameBase
    {
        public OneTimeFrame(Bitmap Bitmap) : base(Bitmap) { }

        public override void Dispose()
        {
            Bitmap.Dispose();
        }
    }
}