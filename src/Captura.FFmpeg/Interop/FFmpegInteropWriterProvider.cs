using System;
using System.Collections;
using System.Collections.Generic;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class FFmpegInteropWriterProvider : IVideoWriterProvider
    {
        public string Name => "FFInterop";

        public IEnumerator<IVideoWriterItem> GetEnumerator()
        {
            yield return new FFmpegInteropItem();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override string ToString() => Name;

        public string Description => @"FF Interop";
    }
}