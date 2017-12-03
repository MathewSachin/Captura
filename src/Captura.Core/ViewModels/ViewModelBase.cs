namespace Captura.ViewModels
{
    public abstract class ViewModelBase : NotifyPropertyChanged
    {
        public Settings Settings => Settings.Instance;
    }
}