using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Captura
{
    public class TranslationSource : NotifyPropertyChanged
    {
        public static TranslationSource Instance { get; } = new TranslationSource();
        
        TranslationSource()
        {
            foreach (var culture in CultureInfo.GetCultures(CultureTypes.AllCultures))
            {
                if (culture.Equals(CultureInfo.InvariantCulture) || culture.Name == "en-US")
                    continue;
                
                if (culture.Name == "en")
                {
                    AvailableCultures.Add(culture);

                    continue;
                }

                try
                {
                    if (Properties.Resources.ResourceManager.GetResourceSet(culture, true, false) != null)
                        AvailableCultures.Add(culture);
                }
                catch
                {
                    // Ignore Culture Not Found and other Exceptions.
                }
            }

            var savedCulture = Settings.Instance.Language;

            CurrentCulture = AvailableCultures.FirstOrDefault(Culture => Culture.Name == savedCulture) ?? new CultureInfo("en");
        }

        public string this[string Key] => Properties.Resources.ResourceManager.GetString(Key, CurrentCulture);

        public ObservableCollection<CultureInfo> AvailableCultures { get; } = new ObservableCollection<CultureInfo>();

        CultureInfo _currentCulture;

        public CultureInfo CurrentCulture
        {
            get => _currentCulture;
            set
            {
                _currentCulture = value;

                Thread.CurrentThread.CurrentUICulture = value;
                
                Settings.Instance.Language = value.Name;

                // ReSharper disable once ExplicitCallerInfoArgument
                OnPropertyChanged("");
            }
        }
    }
}