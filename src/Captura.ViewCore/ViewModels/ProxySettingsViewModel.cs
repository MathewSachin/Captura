using System.Collections.Generic;
using System.Reactive.Linq;
using Captura.Loc;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Captura.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ProxySettingsViewModel : ViewModelBase
    {
        public ProxySettingsViewModel(Settings Settings, ILocalizationProvider Loc)
            : base(Settings, Loc)
        {
            CanAuth = ProxySettings
                .ObserveProperty(M => M.Type)
                .Select(M => M != ProxyType.None)
                .ToReadOnlyReactivePropertySlim();

            CanAuthCred = new[]
                {
                    CanAuth,
                    ProxySettings.ObserveProperty(M => M.Authenticate)
                }
                .CombineLatestValuesAreAllTrue()
                .ToReadOnlyReactivePropertySlim();

            CanHost = ProxySettings
                .ObserveProperty(M => M.Type)
                .Select(M => M == ProxyType.Manual)
                .ToReadOnlyReactivePropertySlim();
        }

        public ProxySettings ProxySettings => Settings.Proxy;

        public IReadOnlyReactiveProperty<bool> CanAuth { get; }

        public IReadOnlyReactiveProperty<bool> CanAuthCred { get; }

        public IReadOnlyReactiveProperty<bool> CanHost { get; }

        public IEnumerable<ProxyType> ProxyTypes { get; } = new[] { ProxyType.None, ProxyType.System, ProxyType.Manual };
    }
}