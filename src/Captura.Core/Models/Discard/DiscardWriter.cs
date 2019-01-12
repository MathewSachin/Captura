namespace Captura.Models
{
    public class DiscardWriter : IVideoFileWriter
    {
        public void Dispose() { }

        readonly byte[] _dummyBuffer = { 0 };

        public void WriteFrame(IBitmapFrame Image)
        {
            if (Image is RepeatFrame)
                return;

            using (Image)
            {
                // This fixes Preview showing multiple mouse pointers
                Image.CopyTo(_dummyBuffer, _dummyBuffer.Length);
            }
        }

        public bool SupportsAudio => false;

        public void WriteAudio(byte[] Buffer, int Length) { }
    }
}