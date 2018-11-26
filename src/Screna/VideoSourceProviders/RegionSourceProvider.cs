namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class RegionSourceProvider : VideoSourceProviderBase
    {
        public RegionSourceProvider(LanguageManager Loc,
            IRegionProvider RegionProvider,
            IIconSet Icons) : base(Loc)
        {
            Source = RegionProvider.VideoSource;
            Icon = Icons.Region;
        }

        public override IVideoItem Source { get; }

        public override string Name => Loc.Region;

        public override string Description { get; } = "Record region selected using Region Selector.";

        public override string Icon { get; }
    }
}