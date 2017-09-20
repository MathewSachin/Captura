namespace Captura
{
    public class TextLocalizer : NotifyPropertyChanged
    {
        public TextLocalizer(string LocalizationKey)
        {
            this.LocalizationKey = LocalizationKey;
            
            TranslationSource.Instance.PropertyChanged += (s, e) => RaisePropertyChanged(nameof(Display));
        }
        
        string _key;

        public string LocalizationKey
        {
            get => _key;
            set
            {
                _key = value;

                OnPropertyChanged();

                RaisePropertyChanged(nameof(Display));
            }
        }

        public string Display => ToString();

        public override string ToString() => TranslationSource.Instance[_key];
    }
}
