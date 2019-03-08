using Captura.ViewModels;
using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Captura
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class AboutViewModel : ViewModelBase
    {
        public ICommand HyperlinkCommand { get; }

        public static Version Version { get; }

        public string AppVersion { get; }

        static AboutViewModel()
        {
            Version = ServiceProvider.AppVersion;
        }

        public AboutViewModel(Settings Settings, ILocalizationProvider Loc) : base(Settings, Loc)
        {
            AppVersion = "v" + Version.ToString(3);

            HyperlinkCommand = new DelegateCommand(Link =>
            {
                if (Link is string s)
                {
                    try
                    {
                        Process.Start(s);
                    }
                    catch
                    {
                        // Suppress Errors
                    }
                }
            });
        }
    }
}
