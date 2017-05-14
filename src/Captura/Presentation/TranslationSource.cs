using System.Globalization;

namespace Captura
{
    public class TranslationSource : NotifyPropertyChanged
    {
        public static TranslationSource Instance { get; } = new TranslationSource();
        
        public string this[string Key] => Properties.Resources.ResourceManager.GetString(Key, CurrentCulture);

        CultureInfo currentCulture;

        public CultureInfo CurrentCulture
        {
            get { return currentCulture; }
            set
            {
                if (currentCulture != value)
                {
                    currentCulture = value;

                    OnPropertyChanged();
                }
            }
        }
    }
}