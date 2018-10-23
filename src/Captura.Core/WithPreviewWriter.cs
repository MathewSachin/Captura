using System.Threading.Tasks;
using Screna;

namespace Captura.Models
{
    public class WithPreviewWriter : IVideoFileWriter
    {
        readonly IPreviewWindow _preview;
        public IVideoFileWriter OriginalWriter { get; }

        public WithPreviewWriter(IVideoFileWriter Writer, IPreviewWindow Preview)
        {
            OriginalWriter = Writer;
            _preview = Preview;
        }

        public void Dispose()
        {
            OriginalWriter.Dispose();
            _preview.Dispose();
        }

        public void WriteFrame(IBitmapFrame Image)
        {
            if (Image is RepeatFrame)
            {
                OriginalWriter.WriteFrame(Image);
            }
            else
            {
                var frame = new MultiDisposeFrame(Image, 2);

                OriginalWriter.WriteFrame(frame);
                Task.Run(() => _preview.Display(frame));
            }
        }

        public bool SupportsAudio => OriginalWriter.SupportsAudio;

        public void WriteAudio(byte[] Buffer, int Length)
        {
            OriginalWriter.WriteAudio(Buffer, Length);
        }
    }
}