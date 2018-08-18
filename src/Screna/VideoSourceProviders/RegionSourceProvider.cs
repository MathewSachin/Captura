namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class RegionSourceProvider : VideoSourceProviderBase
    {
        public RegionSourceProvider(LanguageManager Loc, IRegionProvider RegionProvider) : base(Loc)
        {
            Source = RegionProvider.VideoSource;
        }

        public override IVideoItem Source { get; }

        public override string Name => Loc.Region;

        public override string Description => "Record region selected using Region Selector.";
    }
}