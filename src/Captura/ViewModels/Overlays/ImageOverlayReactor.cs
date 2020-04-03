using System.Reactive.Linq;
using Captura.Video;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Captura.ViewModels
{
    public class ImageOverlayReactor
    {
        public ImageOverlayReactor(ImageOverlaySettings Settings)
        {
            Width = Settings
                .ToReactivePropertyAsSynchronized(M => M.ResizeWidth);

            Height = Settings
                .ToReactivePropertyAsSynchronized(M => M.ResizeHeight);

            Opacity = Settings
                .ObserveProperty(M => M.Opacity)
                .Select(M => M / 100.0)
                .ToReadOnlyReactivePropertySlim();
        }

        public IReactiveProperty<int> Width { get; }
        public IReactiveProperty<int> Height { get; }

        public IReadOnlyReactiveProperty<double> Opacity { get; }
    }
}