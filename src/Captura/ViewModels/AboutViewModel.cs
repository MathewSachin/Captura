using System;
using System.Diagnostics;
using System.Windows.Input;
using Captura.Loc;
using Reactive.Bindings;

namespace Captura.ViewModels
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

            HyperlinkCommand = new ReactiveCommand<string>()
                .WithSubscribe(M => Process.Start(M));
        }
    }
}
