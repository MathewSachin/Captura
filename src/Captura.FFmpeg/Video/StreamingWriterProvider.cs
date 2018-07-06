using System.Collections;
using System.Collections.Generic;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class StreamingWriterProvider : IVideoWriterProvider
    {
        public string Name => "Streaming (Alpha)";

        public IEnumerator<IVideoWriterItem> GetEnumerator()
        {
            return StreamingItem.StreamingItems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override string ToString() => Name;
    }
}