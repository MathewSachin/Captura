using System.Threading.Tasks;
using Screna;

namespace Captura.Models
{
    public class WithPreviewWriter : IVideoFileWriter
    {
        readonly IPreviewWindow _preview;
        readonly IVideoFileWriter _writer;

        public WithPreviewWriter(IVideoFileWriter Writer, IPreviewWindow Preview)
        {
            _writer = Writer;
            _preview = Preview;
        }

        public void Dispose()
        {
            _writer.Dispose();
            _preview.Dispose();
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

                _writer.WriteFrame(frame);
                Task.Run(() => _preview.Display(frame));
            }
        }

        public bool SupportsAudio => _writer.SupportsAudio;

        public void WriteAudio(byte[] Buffer, int Length)
        {
            _writer.WriteAudio(Buffer, Length);
        }
    }
}