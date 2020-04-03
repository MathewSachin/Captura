using System.Reactive.Linq;
using System.Windows;
using Captura.Video;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Captura.ViewModels
{
    public class CensorOverlayReactor
    {
        public CensorOverlayReactor(CensorOverlaySettings Settings)
        {
            Width = Settings
                .ToReactivePropertyAsSynchronized(M => M.Width);

            Height = Settings
                .ToReactivePropertyAsSynchronized(M => M.Height);

            Visible = Settings
                .ObserveProperty(M => M.Display)
                .Select(M => M ? Visibility.Visible : Visibility.Collapsed)
                .ToReadOnlyReactivePropertySlim();
        }

        public IReactiveProperty<int> Width { get; }
        public IReactiveProperty<int> Height { get; }

        public IReadOnlyReactiveProperty<Visibility> Visible { get; }
    }
}