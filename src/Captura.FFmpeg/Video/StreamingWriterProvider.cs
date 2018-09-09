using System.Collections;
using System.Collections.Generic;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class StreamingWriterProvider : IVideoWriterProvider
    {
        public string Name => "Stream";

        public IEnumerator<IVideoWriterItem> GetEnumerator()
        {
            return StreamingItem.StreamingItems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override string ToString() => Name;

        public string Description => @"Stream to streaming sites using FFmpeg (Alpha).
API keys can be set on FFmpeg settings page.";
    }
}