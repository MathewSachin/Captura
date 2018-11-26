using System;

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

        public abstract string Description { get; }

        public abstract string Icon { get; }

        public virtual bool OnSelect() => true;

        public virtual void OnUnselect() { }

        public event Action UnselectRequested;

        protected void RequestUnselect()
        {
            UnselectRequested?.Invoke();
        }

        public virtual string Serialize()
        {
            return Source.ToString();
        }

        public abstract bool Deserialize(string Serialized);

        public abstract bool ParseCli(string Arg);
    }
}