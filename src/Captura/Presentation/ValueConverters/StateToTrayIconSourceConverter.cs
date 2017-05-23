using Captura.Models;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Captura
{
    public class StateToTrayIconSourceConverter : IValueConverter
    {
        readonly ImageSource _logoSource = (ImageSource) new ImageSourceConverter().ConvertFromString("pack://application:,,,/Captura.ico");
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((RecorderState)value)
            {
                case RecorderState.Recording:
                    return "record.ico";

                case RecorderState.Paused:
                    return "pause.ico";

                default:
                    return "Captura.ico";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}