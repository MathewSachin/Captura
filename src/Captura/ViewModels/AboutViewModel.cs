using Captura.ViewModels;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Input;

namespace Captura
{
    public class AboutViewModel : ViewModelBase
    {
        public ICommand HyperlinkCommand { get; }

        public static Version Version { get; }

        public string AppVersion { get; }

        static AboutViewModel()
        {
            Version = Assembly.GetExecutingAssembly().GetName().Version;
        }

        public AboutViewModel(Settings Settings, LanguageManager LanguageManager) : base(Settings, LanguageManager)
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
