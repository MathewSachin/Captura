using System;
using System.Drawing;

namespace Screna
{
    public class RepeatFrame : IBitmapFrame
    {
        RepeatFrame() { }

        public Bitmap Bitmap => throw new NotImplementedException();

        public static RepeatFrame Instance { get; } = new RepeatFrame();

        public void Dispose() { }
        
        public IBitmapEditor GetEditor() => throw new NotImplementedException();
    }
}