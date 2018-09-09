namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class FullScreenSourceProvider : VideoSourceProviderBase
    {
        public FullScreenSourceProvider(LanguageManager Loc, FullScreenItem FullScreenItem) : base(Loc)
        {
            Source = FullScreenItem;
        }

        public override IVideoItem Source { get; }

        public override string Name => Loc.FullScreen;
    }
}