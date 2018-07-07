using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Captura.Models;

namespace Captura
{
    public class RecentItemToIconStyleConverter : OneWayConverter
    {
        static object GetIcon(object Value)
        {
            var app = Application.Current;

            switch (Value)
            {
                case RecentItemType.Image:
                    return app.FindResource("IconImage");

                case RecentItemType.Video:
                    return app.FindResource("IconVideo");

                case RecentItemType.Audio:
                    return app.FindResource("IconMusic");

                case RecentItemType.Link:
                    return app.FindResource("IconLink");

                default:
                    return null;
            }
        }

        static Color GetColor(object Value)
        {
            switch (Value)
            {
                case RecentItemType.Image:
                    return Colors.YellowGreen;

                case RecentItemType.Video:
                    return Colors.OrangeRed;

                case RecentItemType.Audio:
                    return Colors.DodgerBlue;

                case RecentItemType.Link:
                    return Colors.MediumPurple;

                default:
                    return Colors.Black;
            }
        }

        public override object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            var icon = GetIcon(Value);

            var style = new Style(typeof(Path), (Style) Application.Current.Resources[typeof(Path)]);

            if (icon != null)
                style.Setters.Add(new Setter(Path.DataProperty, icon));

            style.Setters.Add(new Setter(Shape.FillProperty, new SolidColorBrush(GetColor(Value))));

            return style;
        }
    }
}