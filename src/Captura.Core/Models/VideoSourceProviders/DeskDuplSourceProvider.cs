using System.Collections.Generic;

namespace Captura.Models
{
    public class DeskDuplSourceProvider : VideoSourceProviderBase
    {
        public DeskDuplSourceProvider(LanguageManager Loc) : base(Loc) { }

        public override IEnumerator<IVideoItem> GetEnumerator()
        {
            return ScreenItem.Enumerate(true).GetEnumerator();
        }

        public override string Name => Loc.DesktopDuplication;
    }
}