using Screna;

namespace Captura.Models
{
    public class WithPreviewWriter : IVideoFileWriter
    {
        readonly PreviewWriter _previewWriter = new PreviewWriter();
        readonly IVideoFileWriter _writer;

        public WithPreviewWriter(IVideoFileWriter Writer)
        {
            _writer = Writer;
        }

        public void Dispose()
        {
            _writer.Dispose();
            _previewWriter.Dispose();
        }

        public void WriteFrame(IBitmapFrame Image)
        {
            if (Image is RepeatFrame)
            {
                _writer.WriteFrame(Image);
            }
            else
            {
                var frame = new MultiDisposeFrame(Image, 2);

                _previewWriter.WriteFrame(frame);
                _writer.WriteFrame(frame);
            }
        }

        public bool SupportsAudio => _writer.SupportsAudio;

        public void WriteAudio(byte[] Buffer, int Length)
        {
            _writer.WriteAudio(Buffer, Length);
        }
    }
}