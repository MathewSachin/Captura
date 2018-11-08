using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Captura
{
    public class IsPlayingToButtonStyleConverter : OneWayConverter
    {
        public override object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            if (Value is bool b)
            {
                var icons = ServiceProvider.Get<IIconSet>();

                var icon = Geometry.Parse(b ? icons.Stop : icons.Play);
                var color = b ? Colors.OrangeRed : Colors.LimeGreen;

                return new Style(typeof(ModernButton), (Style) Application.Current.Resources[typeof(ModernButton)])
                {
                    Setters =
                    {
                        new Setter(ModernButton.IconDataProperty, icon),
                        new Setter(Control.ForegroundProperty, new SolidColorBrush(color))
                    }
                };
            }

            return Binding.DoNothing;
        }
    }
}