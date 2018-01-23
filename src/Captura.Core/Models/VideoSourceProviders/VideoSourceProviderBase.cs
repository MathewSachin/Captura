using System.Collections;
using System.Collections.Generic;

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

        public abstract IEnumerator<IVideoItem> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public abstract string Name { get; }
    }
}