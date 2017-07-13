using Captura.ViewModels;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Windows.Input;

namespace Captura
{
    public class AboutViewModel : ViewModelBase
    {
        public ObservableCollection<CultureInfo> Languages { get; }

        public ICommand HyperlinkCommand { get; } = new DelegateCommand(link =>
        {
            Process.Start(link as string);
        });

        public string AppVersion { get; }

        public AboutViewModel()
        {
            Languages = TranslationSource.Instance.AvailableCultures;

            AppVersion = "v" + Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
        }
        
        public CultureInfo Language
        {
            get => TranslationSource.Instance.CurrentCulture;
            set => TranslationSource.Instance.CurrentCulture = value;
        }
    }
}
