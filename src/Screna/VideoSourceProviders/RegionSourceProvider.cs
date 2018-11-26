namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class RegionSourceProvider : VideoSourceProviderBase
    {
        readonly IRegionProvider _regionProvider;

        public RegionSourceProvider(LanguageManager Loc,
            IRegionProvider RegionProvider,
            IIconSet Icons) : base(Loc)
        {
            _regionProvider = RegionProvider;

            Source = RegionProvider.VideoSource;
            Icon = Icons.Region;

            RegionProvider.SelectorHidden += RequestUnselect;
        }

        public override IVideoItem Source { get; }

        public override string Name => Loc.Region;

        public override string Description { get; } = "Record region selected using Region Selector.";

        public override string Icon { get; }

        public override bool OnSelect()
        {
            _regionProvider.SelectorVisible = true;

            return true;
        }

        public override void OnUnselect()
        {
            _regionProvider.SelectorVisible = false;
        }
    }
}