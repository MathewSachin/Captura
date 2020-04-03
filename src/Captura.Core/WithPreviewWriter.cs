using System.Threading.Tasks;

namespace Captura.Video
{
    public class WithPreviewWriter : IVideoFileWriter
    {
        readonly IPreviewWindow _preview;
        public IVideoFileWriter OriginalWriter { get; private set; }

        public WithPreviewWriter(IVideoFileWriter Writer, IPreviewWindow Preview)
        {
            OriginalWriter = Writer;
            _preview = Preview;
        }

        public void Dispose()
        {
            OriginalWriter.Dispose();
            OriginalWriter = null;
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

        public void WriteAudio(byte[] Buffer, int Offset, int Length)
        {
            OriginalWriter.WriteAudio(Buffer, Offset, Length);
        }
    }
}