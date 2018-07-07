using System.Collections.Generic;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class RegionSourceProvider : VideoSourceProviderBase
    {
        readonly IRegionProvider _regionProvider;

        public RegionSourceProvider(LanguageManager Loc, IRegionProvider RegionProvider) : base(Loc)
        {
            _regionProvider = RegionProvider;
        }

        public override IEnumerator<IVideoItem> GetEnumerator()
        {
            yield return _regionProvider.VideoSource;
        }

        public override string Name => Loc.Region;
    }
}