using System.Collections.Generic;
using System.Globalization;

namespace Captura
{
    public class TranslationSource : NotifyPropertyChanged
    {
        public static TranslationSource Instance { get; } = new TranslationSource();
        
        TranslationSource() { }

        public string this[string Key] => LanguageManager.GetString(Key);

        public IEnumerable<CultureInfo> AvailableCultures { get; } = LanguageManager.AvailableCultures;
        
        public CultureInfo CurrentCulture
        {
            get => LanguageManager.CurrentCulture;
            set
            {
                LanguageManager.CurrentCulture = value; 
                
                OnPropertyChanged("");
            }
        }
    }
}