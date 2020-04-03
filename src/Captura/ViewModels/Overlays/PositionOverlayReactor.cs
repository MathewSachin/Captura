using System.Reactive.Linq;
using System.Windows;
using Captura.Video;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Captura.ViewModels
{
    public class PositionOverlayReactor
    {
        public PositionOverlayReactor(PositionedOverlaySettings Settings)
        {
            HAlignment = Settings
                .ObserveProperty(M => M.HorizontalAlignment)
                .Select(M =>
                {
                    switch (M)
                    {
                        case Alignment.Start:
                            return HorizontalAlignment.Left;

                        case Alignment.Center:
                            return HorizontalAlignment.Center;

                        case Alignment.End:
                            return HorizontalAlignment.Right;

                        default:
                            return HorizontalAlignment.Stretch;
                    }
                })
                .ToReadOnlyReactivePropertySlim();

            VAlignment = Settings
                .ObserveProperty(M => M.VerticalAlignment)
                .Select(M =>
                {
                    switch (M)
                    {
                        case Alignment.Start:
                            return VerticalAlignment.Top;

                        case Alignment.Center:
                            return VerticalAlignment.Center;

                        case Alignment.End:
                            return VerticalAlignment.Bottom;

                        default:
                            return VerticalAlignment.Stretch;
                    }
                })
                .ToReadOnlyReactivePropertySlim();

            Margin = Settings
                .ObserveProperty(M => M.HorizontalAlignment)
                .CombineLatest(
                    Settings
                        .ObserveProperty(M => M.VerticalAlignment),
                    Settings
                        .ObserveProperty(M => M.X),
                    Settings
                        .ObserveProperty(M => M.Y),
                    MarginPropSelector)
                .ToReadOnlyReactivePropertySlim();
        }

        static Thickness MarginPropSelector(Alignment HAlign, Alignment VAlign, int X, int Y)
        {
            int left = 0, top = 0, right = 0, bottom = 0;

            switch (HAlign)
            {
                case Alignment.Start:
                case Alignment.Center:
                    left = X;
                    break;

                case Alignment.End:
                    right = X;
                    break;
            }

            switch (VAlign)
            {
                case Alignment.Start:
                case Alignment.Center:
                    top = Y;
                    break;

                case Alignment.End:
                    bottom = Y;
                    break;
            }

            return new Thickness(left, top, right, bottom);
        }

        public IReadOnlyReactiveProperty<VerticalAlignment> VAlignment { get; }
        public IReadOnlyReactiveProperty<HorizontalAlignment> HAlignment { get; }
        public IReadOnlyReactiveProperty<Thickness> Margin { get; }
    }
}