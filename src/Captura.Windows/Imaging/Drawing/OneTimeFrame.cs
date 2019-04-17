using System.Drawing;

namespace Screna
{
    class OneTimeFrame : DrawingFrameBase
    {
        public OneTimeFrame(Bitmap Bitmap) : base(Bitmap) { }

        public override void Dispose()
        {
            Bitmap.Dispose();
        }
    }
}