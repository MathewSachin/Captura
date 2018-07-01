using System.Collections.Generic;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ScreenSourceProvider : VideoSourceProviderBase
    {
        readonly ScreenPickerItem _screenPickerItem;
        readonly FullScreenItem _fullScreenItem;

        public ScreenSourceProvider(LanguageManager Loc, ScreenPickerItem ScreenPickerItem, FullScreenItem FullScreenItem) : base(Loc)
        {
            _screenPickerItem = ScreenPickerItem;
            _fullScreenItem = FullScreenItem;
        }

        public override IEnumerator<IVideoItem> GetEnumerator()
        {
            yield return _fullScreenItem;

            yield return _screenPickerItem;

            foreach (var screen in ScreenItem.Enumerate())
            {
                yield return screen;
            }
        }

        public override string Name => Loc.Screen;
    }
}