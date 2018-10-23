using System.Drawing;
using Captura;

namespace Screna
{
    public class OneTimeFrame : FrameBase
    {
        public OneTimeFrame(Bitmap Bitmap) : base(Bitmap) { }

        public override void Dispose()
        {
            lock (Bitmap)
            {
                _editor?.Destroy();

                Bitmap.Dispose();
            }
        }

        ReusableEditor _editor;

        public override IBitmapEditor GetEditor()
        {
            lock (Bitmap)
            {
                return _editor ?? (_editor = new ReusableEditor(Graphics.FromImage(Bitmap)));
            }
        }
    }
}