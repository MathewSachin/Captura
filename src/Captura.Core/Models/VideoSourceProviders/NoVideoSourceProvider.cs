using System.Collections.Generic;

namespace Captura.Models
{
    public class NoVideoSourceProvider : VideoSourceProviderBase
    {
        public NoVideoSourceProvider(LanguageManager Loc) : base(Loc) { }

        public override IEnumerator<IVideoItem> GetEnumerator()
        {
            yield return WaveItem.Instance;

            foreach (var item in FFMpegAudioItem.Items)
            {
                yield return item;
            }
        }

        public override string Name => Loc.OnlyAudio;
    }
}