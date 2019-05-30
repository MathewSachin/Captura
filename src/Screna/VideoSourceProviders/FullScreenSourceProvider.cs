using Screna;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class FullScreenSourceProvider : VideoSourceProviderBase
    {
        public FullScreenSourceProvider(ILocalizationProvider Loc,
            IIconSet Icons,
            IPlatformServices PlatformServices) : base(Loc)
        {
            Source = new FullScreenItem(PlatformServices);
            Icon = Icons.MultipleMonitor;
        }

        public override IVideoItem Source { get; }

        public override string Name => Loc.FullScreen;

        public override string Description { get; } = "Record Fullscreen.";

        public override string Icon { get; }

        public override string Serialize() => "";

        public override bool Deserialize(string Serialized) => true;

        public override bool ParseCli(string Arg)
        {
            return string.IsNullOrWhiteSpace(Arg) || Arg == "desktop";
        }

        public override IBitmapImage Capture(bool IncludeCursor)
        {
            return ScreenShot.Capture(IncludeCursor);
        }
    }
}