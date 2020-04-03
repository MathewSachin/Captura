using SharpAvi.Codecs;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Captura.Video;

namespace Captura.SharpAvi
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

        public IVideoWriterItem ParseCli(string Cli)
        {
            if (ServiceProvider.FileExists("SharpAvi.dll") && Regex.IsMatch(Cli, @"^sharpavi:\d+$"))
            {
                var index = int.Parse(Cli.Substring(9));

                var writers = this.ToArray();

                if (index < writers.Length)
                {
                    return writers[index];
                }
            }

            return null;
        }

        public string Description => "Encode Avi videos using SharpAvi.";
    }
}