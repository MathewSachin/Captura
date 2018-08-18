using System.Collections.Generic;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ScreenSourceProvider : VideoSourceProviderBase
    {
        readonly ScreenPickerItem _screenPickerItem;
        
        public ScreenSourceProvider(LanguageManager Loc, ScreenPickerItem ScreenPickerItem) : base(Loc)
        {
            _screenPickerItem = ScreenPickerItem;
        }

        public override IEnumerator<IVideoItem> GetEnumerator()
        {
            yield return _screenPickerItem;

            foreach (var screen in ScreenItem.Enumerate())
            {
                yield return screen;
            }
        }

        public override string Name => Loc.Screen;

        public override string Description => "Record a specific screen.";
    }
}