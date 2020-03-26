using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Point = System.Drawing.Point;

namespace Captura.ViewModels
{
    public class WebcamOverlayReactor
    {
        public WebcamOverlayReactor(WebcamOverlaySettings Settings)
        {
            Location = Settings.ObserveProperty(M => M.XLoc)
                .CombineLatest(
                    Settings.ObserveProperty(M => M.YLoc),
                    FrameSize,
                    WebcamSize,
                    Settings.ObserveProperty(M => M.Scale),
                    (X, Y, FrameSize, WebcamSize, Scale) => Settings.GetPosition(FrameSize.ToDrawingSize(), WebcamSize.ToDrawingSize()))
                .ToReadOnlyReactivePropertySlim();

            Size = Settings.ObserveProperty(M => M.Scale)
                .CombineLatest(
                    FrameSize,
                    WebcamSize,
                    (Scale, FrameSize, WebcamSize) => Settings.GetSize(FrameSize.ToDrawingSize(), WebcamSize.ToDrawingSize()).ToWpfSize())
                .ToReadOnlyReactivePropertySlim();

            Opacity = Settings
                .ObserveProperty(M => M.Opacity)
                .Select(M => M / 100.0)
                .ToReadOnlyReactivePropertySlim();
        }

        public Subject<Size> FrameSize { get; } = new Subject<Size>();

        public Subject<Size> WebcamSize { get; } = new Subject<Size>();

        public IReadOnlyReactiveProperty<Size> Size { get; }

        public IReadOnlyReactiveProperty<double> Opacity { get; }

        public IReadOnlyReactiveProperty<Point> Location { get; }
    }
}