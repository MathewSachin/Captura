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
            Margin = Settings
                .ObserveProperty(M => M.Left)
                .CombineLatest(
                    Settings
                        .ObserveProperty(M => M.Top),
                    (L, T) =>
                    {
                        if (double.IsNaN(Control.ActualWidth) || double.IsNaN(Control.ActualHeight))
                            return new Thickness();

                        var x = Settings.GetX(FullWidth, Control.ActualWidth);
                        var y = Settings.GetY(FullHeight, Control.ActualHeight);

                        return new Thickness(x, y, 0, 0);
                    })
                .ToReactiveProperty();

            Margin.Subscribe(M =>
            {
                if (double.IsNaN(Control.ActualWidth) || double.IsNaN(Control.ActualHeight))
                    return;

                Settings.Left = Settings.ToSetX(FullWidth, Control.ActualWidth, M.Left);
                Settings.Top = Settings.ToSetX(FullHeight, Control.ActualHeight, M.Top);
            });
        }

        public IReactiveProperty<Thickness> Margin { get; }
    }
}