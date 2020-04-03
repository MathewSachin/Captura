using Captura.Loc;

namespace Captura.ViewModels
{
    public abstract class ViewModelBase : NotifyPropertyChanged
    {
        protected ViewModelBase(Settings Settings, ILocalizationProvider Loc)
        {
            this.Settings = Settings;
            this.Loc = Loc;
        }

        public Settings Settings { get; }

        public ILocalizationProvider Loc { get; }
    }
}