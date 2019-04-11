using System.Drawing;
using Screna;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class RegionSourceProvider : VideoSourceProviderBase
    {
        readonly IRegionProvider _regionProvider;
        readonly IPlatformServices _platformServices;

        public RegionSourceProvider(ILocalizationProvider Loc,
            IRegionProvider RegionProvider,
            IIconSet Icons,
            IPlatformServices PlatformServices) : base(Loc)
        {
            _regionProvider = RegionProvider;
            _platformServices = PlatformServices;

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

            var selectedRegion = _regionProvider.SelectedRegion;
            var fullScreen = _platformServices.DesktopRectangle;

            // Fully outside all screens, reset location
            if (Rectangle.Intersect(selectedRegion, fullScreen) == Rectangle.Empty)
            {
                _regionProvider.SelectedRegion = new Rectangle(50, 50, 500, 500);
            }

            return true;
        }

        public override void OnUnselect()
        {
            _regionProvider.SelectorVisible = false;
        }

        public override string Serialize()
        {
            var rect = _regionProvider.SelectedRegion;
            return rect.ConvertToString();
        }

        public override bool Deserialize(string Serialized)
        {
            if (!(Serialized.ConvertToRectangle() is Rectangle rect))
                return false;

            _regionProvider.SelectedRegion = rect;

            OnSelect();

            return true;
        }

        public override bool ParseCli(string Arg)
        {
            if (!(Arg.ConvertToRectangle() is Rectangle rect))
                return false;

            _regionProvider.SelectedRegion = rect.Even();

            return true;
        }
    }
}