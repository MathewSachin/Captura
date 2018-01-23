using System.Collections.Generic;

namespace Captura.Models
{
    public class ScreenSourceProvider : VideoSourceProviderBase
    {
        public ScreenSourceProvider(LanguageManager Loc) : base(Loc) { }

        public override IEnumerator<IVideoItem> GetEnumerator()
        {
            yield return FullScreenItem.Instance;

            foreach (var screen in ScreenItem.Enumerate(false))
            {
                yield return screen;
            }
        }

        public override string Name => Loc.Screen;
    }
}