namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class FullScreenSourceProvider : VideoSourceProviderBase
    {
        public FullScreenSourceProvider(LanguageManager Loc,
            IIconSet Icons,
            // ReSharper disable once SuggestBaseTypeForParameter
            FullScreenItem FullScreenItem) : base(Loc)
        {
            Source = FullScreenItem;
            Icon = Icons.MultipleMonitor;
        }

        public override IVideoItem Source { get; }

        public override string Name => Loc.FullScreen;

        public override string Description { get; } = "Record Fullscreen.";

        public override string Icon { get; }

        public override string Serialize() => "";

        public override bool Deserialize(string Serialized) => true;
    }
}