using Screna;

namespace Captura.Models
{
    public class DiscardWriter : IVideoFileWriter
    {
        public void Dispose() { }

        public void WriteFrame(IBitmapFrame Image)
        {
            Image.Dispose();
        }

        public bool SupportsAudio { get; }

        public void WriteAudio(byte[] Buffer, int Length) { }
    }
}