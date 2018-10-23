using System;
using System.IO;

namespace Captura
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

        public int Width { get; } = -1;
        public int Height { get; } = -1;

        public void CopyTo(byte[] Buffer, int Length)
        {
            throw new NotImplementedException();
        }

        public IBitmapEditor GetEditor() => throw new NotImplementedException();
    }
}