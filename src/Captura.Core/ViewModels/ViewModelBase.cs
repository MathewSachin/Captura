namespace Captura.ViewModels
{
    public abstract class ViewModelBase : NotifyPropertyChanged
    {
        protected ViewModelBase(Settings Settings, LanguageManager LanguageManager)
        {
            this.Settings = Settings;
            Loc = LanguageManager;
        }

        public Settings Settings { get; }

        public LanguageManager Loc { get; }
    }
}