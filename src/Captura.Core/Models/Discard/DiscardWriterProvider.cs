using System.Collections;
using System.Collections.Generic;

namespace Captura.Models
{
    public class DiscardWriterProvider : IVideoWriterProvider
    {
        public IEnumerator<IVideoWriterItem> GetEnumerator()
        {
            yield return new DiscardWriterItem();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public string Name { get; } = "Discard";
    }
}