using System.Collections.Generic;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class NoVideoSourceProvider : VideoSourceProviderBase
    {
        public NoVideoSourceProvider(LanguageManager Loc) : base(Loc) { }

        public override IEnumerator<IVideoItem> GetEnumerator()
        {
            yield return WaveItem.Instance;

            foreach (var item in FFmpegAudioItem.Items)
            {
                yield return item;
            }
        }

        public override string Name => Loc.OnlyAudio;

        public override string Description =>
            @"No Video recorded.
Can be used for audio-only recording.";
    }
}