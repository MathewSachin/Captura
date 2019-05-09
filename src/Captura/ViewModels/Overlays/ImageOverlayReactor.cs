using System.Drawing;
using System.Reactive.Linq;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Captura.ViewModels
{
    public class ImageOverlayReactor
    {
        public ImageOverlayReactor(ImageOverlaySettings Settings, double FullWidth, double FullHeight)
        {
            Width = Settings
                .ToReactivePropertyAsSynchronized(M => M.Width,
                    M => Settings.GetWidth(FullWidth),
                    M => Settings.ToSetWidth(FullWidth, M));

            Height = Settings
                .ToReactivePropertyAsSynchronized(M => M.Height,
                    M => Settings.GetHeight(FullHeight),
                    M => Settings.ToSetHeight(FullHeight, M));

            Opacity = Settings
                .ObserveProperty(M => M.Opacity)
                .Select(M => M / 100.0)
                .ToReadOnlyReactivePropertySlim();
        }

        public IReactiveProperty<double> Width { get; }
        public IReactiveProperty<double> Height { get; }

        public IReadOnlyReactiveProperty<double> Opacity { get; }
    }
}