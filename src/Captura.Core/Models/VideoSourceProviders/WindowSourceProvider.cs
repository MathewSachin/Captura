using System.Collections.Generic;
using Screna;

namespace Captura.Models
{
    public class WindowSourceProvider : VideoSourceProviderBase
    {
        public WindowSourceProvider(LanguageManager Loc) : base(Loc) { }

        public override IEnumerator<IVideoItem> GetEnumerator()
        {
            yield return WindowItem.TaskBar;

            foreach (var win in Window.EnumerateVisible())
                yield return new WindowItem(win);
        }

        public override string Name => Loc.Window;
    }
}