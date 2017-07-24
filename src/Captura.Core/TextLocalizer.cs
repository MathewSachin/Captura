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
            // 키 값이 한글로 들어 올때가 있음
            var str = Resources.ResourceManager.GetString(_key, null);
            if (_key != null && str == null)
            {
                str = _key;
            }
            return str;
        }
    }
}
