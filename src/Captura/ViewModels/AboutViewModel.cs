using Captura.ViewModels;
using System;
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
            var s = link as string;

            if (s != null)
                Process.Start(s);
        });

        public static Version Version { get; }

        public string AppVersion { get; }

        static AboutViewModel()
        {
            Version = Assembly.GetExecutingAssembly().GetName().Version;
        }

        public AboutViewModel()
        {
            Languages = TranslationSource.Instance.AvailableCultures;

            AppVersion = "v" + Version.ToString(3);
        }
        
        public CultureInfo Language
        {
            get => TranslationSource.Instance.CurrentCulture;
            set => TranslationSource.Instance.CurrentCulture = value;
        }
    }
}
