using Captura.ViewModels;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Captura
{
    public class AboutViewModel : ViewModelBase
    {
        public ObservableCollection<CultureInfo> Languages { get; }

        public AboutViewModel()
        {
            Languages = TranslationSource.Instance.AvailableCultures;            
        }
        
        public CultureInfo Language
        {
            get => TranslationSource.Instance.CurrentCulture;
            set => TranslationSource.Instance.CurrentCulture = value;
        }
    }
}
