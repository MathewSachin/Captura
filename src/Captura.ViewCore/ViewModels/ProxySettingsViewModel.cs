using System.Collections.Generic;

namespace Captura.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ProxySettingsViewModel : ViewModelBase
    {
        public ProxySettingsViewModel(Settings Settings, LanguageManager LanguageManager)
            : base(Settings, LanguageManager)
        {
            Settings.Proxy.PropertyChanged += (S, E) => RaiseAllChanged();
        }

        public ProxySettings ProxySettings => Settings.Proxy;

        public bool CanAuth => ProxySettings.Type != ProxyType.None;

        public bool CanAuthCred => CanAuth && ProxySettings.Authenticate;

        public bool CanHost => ProxySettings.Type == ProxyType.Manual;

        public IEnumerable<ProxyType> ProxyTypes { get; } = new[] { ProxyType.None, ProxyType.System, ProxyType.Manual };
    }
}