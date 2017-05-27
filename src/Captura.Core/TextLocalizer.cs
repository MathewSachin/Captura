using Captura.Properties;

namespace Captura
{
    public class TextLocalizer : NotifyPropertyChanged
    {
        public TextLocalizer(string LocalizationKey)
        {
            this.LocalizationKey = LocalizationKey;

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
                OnPropertyChanged(nameof(Display));
            }
        }

        public string Display => ToString();

        public override string ToString()
        {
            return Resources.ResourceManager.GetString(_key, null);
        }
    }
}
