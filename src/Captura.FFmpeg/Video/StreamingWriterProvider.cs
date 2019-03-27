using System.Collections;
using System.Collections.Generic;
using Captura.FFmpeg;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class StreamingWriterProvider : IVideoWriterProvider
    {
        public string Name => "Stream";

        public IEnumerator<IVideoWriterItem> GetEnumerator()
        {
            yield return new TwitchVideoCodec();
            yield return new YouTubeLiveVideoCodec();
            yield return new CustomStreamingVideoCodec();
        }

        public static IVideoWriterItem GetCustomStreamingCodec() => new CustomStreamingVideoCodec();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override string ToString() => Name;

        public string Description => @"Stream to streaming sites using FFmpeg (Alpha).
API keys can be set on FFmpeg settings page.";
    }
}