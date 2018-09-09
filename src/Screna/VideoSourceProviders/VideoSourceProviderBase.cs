namespace Captura.Models
{
    public abstract class VideoSourceProviderBase : NotifyPropertyChanged, IVideoSourceProvider
    {
        protected readonly LanguageManager Loc;

        protected VideoSourceProviderBase(LanguageManager Loc)
        {
            this.Loc = Loc;

            Loc.LanguageChanged += L => RaisePropertyChanged(nameof(Name));
        }

        public abstract IVideoItem Source { get; }

        public abstract string Name { get; }
    }
}