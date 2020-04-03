using System.Reactive.Linq;
using System.Windows;
using System.Windows.Media;
using Captura.Video;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Captura.ViewModels
{
    public class TextOverlayReactor
    {
        public TextOverlayReactor(TextOverlaySettings Settings)
        {
            FontFamily = Settings
                .ObserveProperty(M => M.FontFamily)
                .Select(M => new FontFamily(M))
                .ToReadOnlyReactivePropertySlim();

            FontSize = Settings
                .ObserveProperty(M => M.FontSize)
                .ToReadOnlyReactivePropertySlim();

            Padding = new[]
                {
                    Settings
                        .ObserveProperty(M => M.HorizontalPadding),
                    Settings
                        .ObserveProperty(M => M.VerticalPadding)
                }
                .CombineLatest(M =>
                {
                    var w = M[0];
                    var h = M[1];

                    return new Thickness(w, h, w, h);
                })
                .ToReadOnlyReactivePropertySlim();

            Foreground = Settings
                .ObserveProperty(M => M.FontColor)
                .Select(M => new SolidColorBrush(M.ToWpfColor()))
                .ToReadOnlyReactivePropertySlim();

            Background = Settings
                .ObserveProperty(M => M.BackgroundColor)
                .Select(M => new SolidColorBrush(M.ToWpfColor()))
                .ToReadOnlyReactivePropertySlim();

            BorderThickness = Settings
                .ObserveProperty(M => M.BorderThickness)
                .Select(M => new Thickness(M))
                .ToReadOnlyReactivePropertySlim();

            BorderBrush = Settings
                .ObserveProperty(M => M.BorderColor)
                .Select(M => new SolidColorBrush(M.ToWpfColor()))
                .ToReadOnlyReactivePropertySlim();

            CornerRadius = Settings
                .ObserveProperty(M => M.CornerRadius)
                .Select(M => new CornerRadius(M))
                .ToReadOnlyReactivePropertySlim();
        }

        public IReadOnlyReactiveProperty<FontFamily> FontFamily { get; }

        public IReadOnlyReactiveProperty<int> FontSize { get; }

        public IReadOnlyReactiveProperty<Thickness> Padding { get; }

        public IReadOnlyReactiveProperty<Brush> Foreground { get; }

        public IReadOnlyReactiveProperty<Brush> Background { get; }

        public IReadOnlyReactiveProperty<Thickness> BorderThickness { get; }

        public IReadOnlyReactiveProperty<Brush> BorderBrush { get; }

        public IReadOnlyReactiveProperty<CornerRadius> CornerRadius { get; }
    }
}