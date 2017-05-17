using Captura.Properties;

namespace Captura
{
    public class TextLocalizer : NotifyPropertyChanged
    {
        public TextLocalizer(string LocalizationKey)
        {
            _key = LocalizationKey;

            TranslationSource.Instance.PropertyChanged += (s, e) => OnPropertyChanged(nameof(Display));
        }

        readonly string _key;

        public string Display => ToString();

        public override string ToString()
        {
            return Resources.ResourceManager.GetString(_key, null);
        }
    }
}
