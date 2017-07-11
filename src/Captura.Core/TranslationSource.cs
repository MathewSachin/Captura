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
            foreach (var culture in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
            {
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

            if (!AvailableCultures.Any(culture => culture.Name == "en-US"))
                AvailableCultures.Insert(0, new CultureInfo("en-US"));

            var savedCulture = Settings.Instance.Language;

            CurrentCulture = new CultureInfo(AvailableCultures.Any(culture => culture.Name == savedCulture) ? savedCulture : "en-US");
        }

        public string this[string Key] => Properties.Resources.ResourceManager.GetString(Key, CurrentCulture);

        public ObservableCollection<CultureInfo> AvailableCultures { get; } = new ObservableCollection<CultureInfo>();

        CultureInfo currentCulture;

        public CultureInfo CurrentCulture
        {
            get => currentCulture;
            set
            {
                currentCulture = value;

                Thread.CurrentThread.CurrentUICulture = value;
                
                Settings.Instance.Language = value.Name;

                OnPropertyChanged("");
            }
        }
    }
}