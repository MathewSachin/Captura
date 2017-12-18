using System;
using System.Drawing;

namespace Screna
{
    public class ReusableFrame : FrameBase
    {
        readonly ReusableEditor _editor;

        public ReusableFrame(Bitmap Bitmap) : base(Bitmap)
        {
            _editor = new ReusableEditor(Graphics.FromImage(Bitmap));
        }

        public override void Dispose()
        {
            Released?.Invoke();
        }

        public override IBitmapEditor GetEditor() => _editor;

        public void Destroy()
        {
            _editor.Destroy();

            lock (Bitmap)
            {
                Bitmap.Dispose();
            }
        }

        public event Action Released;
    }
}