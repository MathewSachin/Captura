namespace Captura.ViewModels
{
    public abstract class ViewModelBase : NotifyPropertyChanged
    {
        protected ViewModelBase(Settings Settings)
        {
            this.Settings = Settings;
        }

        public Settings Settings { get; }
    }
}