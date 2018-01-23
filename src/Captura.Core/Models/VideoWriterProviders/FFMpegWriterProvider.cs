using System.Collections;
using System.Collections.Generic;

namespace Captura.Models
{
    public class FFMpegWriterProvider : IVideoWriterProvider
    {
        public string Name => "FFMpeg";

        public IEnumerator<IVideoWriterItem> GetEnumerator()
        {
            return FFMpegItem.Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override string ToString() => Name;
    }
}