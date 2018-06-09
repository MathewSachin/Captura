using System.Collections.Generic;

namespace Captura.Models
{
    public class ScreenSourceProvider : VideoSourceProviderBase
    {
        readonly ScreenPickerItem _screenPickerItem;

        public ScreenSourceProvider(LanguageManager Loc, ScreenPickerItem ScreenPickerItem) : base(Loc)
        {
            _screenPickerItem = ScreenPickerItem;
        }

        public override IEnumerator<IVideoItem> GetEnumerator()
        {
            yield return FullScreenItem.Instance;

            yield return _screenPickerItem;

            foreach (var screen in ScreenItem.Enumerate(false))
            {
                yield return screen;
            }
        }

        public override string Name => Loc.Screen;
    }
}