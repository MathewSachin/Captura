using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Captura.ViewModels
{
    public class PositionOverlayReactor
    {
        public PositionOverlayReactor(PositionedOverlaySettings Settings, double FullWidth, double FullHeight, Control Control)
        {
            bool CheckNaN()
            {
                return double.IsNaN(Control.ActualWidth) || double.IsNaN(Control.ActualHeight);
            }

            Margin = Settings
                .PropertyChangedAsObservable()
                .ToUnit()
                .Merge(Observable.Timer(TimeSpan.FromMilliseconds(100)).ToUnit())
                .Where(M => !CheckNaN())
                .Select(_ =>
                {
                    var x = Settings.GetX(FullWidth, Control.ActualWidth);
                    var y = Settings.GetY(FullHeight, Control.ActualHeight);

                    return new Thickness(x, y, 0, 0);
                })
                .ToReadOnlyReactivePropertySlim();
        }

        public IReadOnlyReactiveProperty<Thickness> Margin { get; }
    }
}