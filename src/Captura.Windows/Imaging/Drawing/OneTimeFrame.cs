using System;
using System.Drawing;

namespace Screna
{
    class OneTimeFrame : DrawingFrameBase
    {
        public OneTimeFrame(Bitmap Bitmap, TimeSpan Timestamp) : base(Bitmap, Timestamp) { }

        public override void Dispose()
        {
            Bitmap.Dispose();
        }
    }
}