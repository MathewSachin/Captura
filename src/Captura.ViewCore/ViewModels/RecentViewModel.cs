using Captura.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Captura.Loc;
using Reactive.Bindings;

namespace Captura.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class RecentViewModel : ViewModelBase
    {
        public ReadOnlyObservableCollection<IRecentItem> Items { get; }

        public ICommand ClearCommand { get; }

        public RecentViewModel(Settings Settings,
            ILocalizationProvider Loc,
            IRecentList Recent)
            : base(Settings, Loc)
        {
            Items = Recent.Items;

            ClearCommand = new ReactiveCommand()
                .WithSubscribe(Recent.Clear);
        }
    }
}