using SharpAvi.Codecs;
using System.Collections;
using System.Collections.Generic;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class SharpAviWriterProvider : IVideoWriterProvider
    {
        public string Name => "SharpAvi";

        public IEnumerator<IVideoWriterItem> GetEnumerator()
        {
            yield return new SharpAviItem(AviCodec.MotionJpeg, "Motion JPEG encoder using WPF's JPG encoder");
            yield return new SharpAviItem(AviCodec.Uncompressed, "Uncompressed Avi");
            yield return new SharpAviItem(AviCodec.Lagarith, "Lagarith codec needs to be installed manually and configured to use RGB mode with Null Frames disabled.");

            foreach (var codec in Mpeg4VideoEncoderVcm.GetAvailableCodecs())
                yield return new SharpAviItem(new AviCodec(codec.Codec, codec.Name), "");
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override string ToString() => Name;

        public string Description => "Encode Avi videos using SharpAvi.";
    }
}