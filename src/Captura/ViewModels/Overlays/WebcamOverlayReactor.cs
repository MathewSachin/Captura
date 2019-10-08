using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Captura.ViewModels
{
    public class WebcamOverlayReactor
    {
        public WebcamOverlayReactor(WebcamOverlaySettings Settings)
        {
            Margin = Settings
                .ObserveProperty(M => M.X)
                .CombineLatest(
                    Settings
                        .ObserveProperty(M => M.Y),
                    FrameSize,
                    (X, Y, FrameSize) =>
                    {
                        var pos = Settings.GetPosition((float)FrameSize.Width, (float)FrameSize.Height);

                        return new Thickness(pos.X, pos.Y, 0, 0);
                    })
                .ToReadOnlyReactivePropertySlim();

            Width = Settings
                .ObserveProperty(M => M.Width)
                .CombineLatest(FrameSize, (Width, FrameSize) => Settings.GetWidth((float)FrameSize.Width))
                .ToReadOnlyReactivePropertySlim();

            Height = Settings
                .ObserveProperty(M => M.Height)
                .CombineLatest(FrameSize, (Height, FrameSize) => Settings.GetHeight((float)FrameSize.Height))
                .ToReadOnlyReactivePropertySlim();

            Opacity = Settings
                .ObserveProperty(M => M.Opacity)
                .Select(M => M / 100.0)
                .ToReadOnlyReactivePropertySlim();
        }

        public Subject<Size> FrameSize { get; } = new Subject<Size>();

        public IReadOnlyReactiveProperty<float> Width { get; }
        public IReadOnlyReactiveProperty<float> Height { get; }

        public IReadOnlyReactiveProperty<double> Opacity { get; }

        public IReadOnlyReactiveProperty<Thickness> Margin { get; }
    }
}