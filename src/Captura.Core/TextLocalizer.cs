namespace Captura
{
    public class TextLocalizer : NotifyPropertyChanged
    {
        public TextLocalizer(string LocalizationKey)
        {
            this.LocalizationKey = LocalizationKey;

            // ReSharper disable once ExplicitCallerInfoArgument
            TranslationSource.Instance.PropertyChanged += (s, e) => OnPropertyChanged(nameof(Display));
        }
        
        string _key;

        public string LocalizationKey
        {
            get => _key;
            set
            {
                _key = value;

                OnPropertyChanged();

                // ReSharper disable once ExplicitCallerInfoArgument
                OnPropertyChanged(nameof(Display));
            }
        }

        public string Display => ToString();

        public override string ToString() => TranslationSource.Instance[_key];
    }
}
