using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Captura.Models
{
    public class SharpAviWriterProvider : IVideoWriterProvider
    {
        public string Name => "SharpAvi";

        public IEnumerator<IVideoWriterItem> GetEnumerator()
        {
            return AviWriter.EnumerateEncoders().Select(Codec => new SharpAviItem(Codec)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override string ToString() => Name;
    }
}