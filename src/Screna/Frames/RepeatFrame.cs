using System;
using System.IO;

namespace Screna
{
    public class RepeatFrame : IBitmapFrame
    {
        RepeatFrame() { }

        public static RepeatFrame Instance { get; } = new RepeatFrame();

        public void Dispose() { }

        public void SaveGif(Stream Stream)
        {
            throw new NotImplementedException();
        }

        public int Width { get; }
        public int Height { get; }

        public void CopyTo(byte[] Buffer, int Length)
        {
            throw new NotImplementedException();
        }

        public IBitmapEditor GetEditor() => throw new NotImplementedException();
    }
}