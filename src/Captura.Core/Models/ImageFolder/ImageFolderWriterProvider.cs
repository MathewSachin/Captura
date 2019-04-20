using System.Collections;
using System.Collections.Generic;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ImageFolderWriterProvider : IVideoWriterProvider
    {
        public IEnumerator<IVideoWriterItem> GetEnumerator()
        {
            yield return new ImageFolderWriterItem();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public string Name { get; } = "Preview Only";

        public string Description => "For testing purposes.";

        public override string ToString() => Name;
    }
}