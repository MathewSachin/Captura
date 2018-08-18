using System.Collections.Generic;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class FullScreenSourceProvider : VideoSourceProviderBase
    {
        readonly FullScreenItem _fullScreenItem;

        public FullScreenSourceProvider(LanguageManager Loc, FullScreenItem FullScreenItem) : base(Loc)
        {
            _fullScreenItem = FullScreenItem;
        }

        public override IEnumerator<IVideoItem> GetEnumerator()
        {
            yield return _fullScreenItem;
        }

        public override string Name => Loc.FullScreen;

        public override string Description => "Record Fullscreen.";
    }
}