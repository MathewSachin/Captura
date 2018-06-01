using System.Collections;
using System.Collections.Generic;

namespace Captura.Models
{
    public class FFMpegWriterProvider : IVideoWriterProvider
    {
        public string Name => "FFMpeg";

        readonly Settings _settings;

        public FFMpegWriterProvider(Settings Settings)
        {
            _settings = Settings;
        }

        public IEnumerator<IVideoWriterItem> GetEnumerator()
        {
            foreach (var codec in FFMpegItem.Items)
            {
                yield return codec;
            }

            foreach (var item in _settings.FFMpeg.CustomCodecs)
            {
                yield return new FFMpegItem(item);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override string ToString() => Name;
    }
}