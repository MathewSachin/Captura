using System.Drawing;
using System.Text.RegularExpressions;
using Screna;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class RegionSourceProvider : VideoSourceProviderBase
    {
        readonly IRegionProvider _regionProvider;
        readonly IPlatformServices _platformServices;
        static readonly RectangleConverter RectangleConverter = new RectangleConverter();

        public RegionSourceProvider(LanguageManager Loc,
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
            return RectangleConverter.ConvertToInvariantString(rect);
        }

        public override bool Deserialize(string Serialized)
        {
            if (!(RectangleConverter.ConvertFromInvariantString(Serialized) is Rectangle rect))
                return false;

            _regionProvider.SelectedRegion = rect;

            OnSelect();

            return true;
        }

        public override bool ParseCli(string Arg)
        {
            if (!Regex.IsMatch(Arg, @"^\d+,\d+,\d+,\d+$"))
                return false;

            var rectConverter = new RectangleConverter();

            if (!(rectConverter.ConvertFromInvariantString(Arg) is Rectangle rect))
                return false;

            _regionProvider.SelectedRegion = rect.Even();

            return true;
        }
    }
}