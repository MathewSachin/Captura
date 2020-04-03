namespace Captura.Loc
{
    public class TextLocalizer : NotifyPropertyChanged
    {
        public TextLocalizer(string LocalizationKey)
        {
            this.LocalizationKey = LocalizationKey;

            LanguageManager.Instance.LanguageChanged += L => RaisePropertyChanged(nameof(Display));
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

        public override string ToString() => LanguageManager.Instance[_key];
    }
}
