namespace Captura
{
    public class LanguageField : NotifyPropertyChanged
    {
        readonly TranslationViewModel _translationViewModel;

        public LanguageField(string Key, TranslationViewModel TranslationViewModel)
        {
            this.Key = Key;

            _translationViewModel = TranslationViewModel;

            _translationViewModel.PropertyChanged += (S, E) => RaisePropertyChanged(nameof(Value));
        }

        public string Key { get; }

        public string Value
        {
            get => _translationViewModel[Key];
            set
            {
                _translationViewModel[Key] = value;
                
                OnPropertyChanged();
            }
        }
    }
}