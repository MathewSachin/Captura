using Captura.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Captura
{
    public class MediaTypeToPlayButtonVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var mediaType = MediaType.Image;
            if (Enum.TryParse<MediaType>(value.ToString(), out mediaType))
            {
                if(mediaType == MediaType.Image)
                {
                    return Visibility.Hidden; 
                }
                else
                {
                    return Visibility.Visible;
                }
            }
            else
            {
                return Binding.DoNothing;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                //case string s:
                //    return ColorTranslator.FromHtml(s);

                //case WpfColor c:
                //    return Color.FromArgb(c.A, c.R, c.G, c.B);

                default:
                    return Binding.DoNothing;
            }
        }
    }
}
