using System.Collections;
using System.Collections.Generic;

namespace Captura.Models
{
    public class PreviewWriterProvider : IVideoWriterProvider
    {
        public IEnumerator<IVideoWriterItem> GetEnumerator()
        {
            yield return new PreviewWriterItem();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public string Name { get; } = "Preview";
    }
}