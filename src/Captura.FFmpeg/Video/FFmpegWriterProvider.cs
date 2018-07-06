using System.Collections;
using System.Collections.Generic;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class FFmpegWriterProvider : IVideoWriterProvider
    {
        public string Name => "FFmpeg";

        readonly FFmpegSettings _settings;

        public FFmpegWriterProvider(FFmpegSettings Settings)
        {
            _settings = Settings;
        }

        public IEnumerator<IVideoWriterItem> GetEnumerator()
        {
            foreach (var codec in FFmpegItem.Items)
            {
                yield return codec;
            }

            foreach (var codec in FFmpegPostProcessingItem.Items)
            {
                yield return codec;
            }

            foreach (var item in _settings.CustomCodecs)
            {
                yield return new FFmpegItem(item);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override string ToString() => Name;
    }
}