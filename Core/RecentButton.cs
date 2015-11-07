using System.Diagnostics;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Captura
{
    public class RecentButton : Button
    {
        static readonly SolidColorBrush AviBack = new SolidColorBrush(Colors.Black),
            GifBack = new SolidColorBrush(Colors.DarkGreen),
            WavBack = new SolidColorBrush(Colors.DarkOrange),
            OtherBack = new SolidColorBrush(Colors.Chocolate);

        public RoutedUICommand RecentButtonClick = new RoutedUICommand();

        public RecentButton(string FilePath)
        {
            Content = Path.GetFileName(FilePath);
            Foreground = GetColor(FilePath);

            DataContext = this;

            CommandBindings.Add(new CommandBinding(RecentButtonClick, (s, e) => Process.Start(FilePath),
                (s, e) => e.CanExecute = File.Exists(FilePath)));

            Command = RecentButtonClick;
        }

        static SolidColorBrush GetColor(string FilePath)
        {
            switch (Path.GetExtension(FilePath))
            {
                case ".avi":
                    return AviBack;

                case ".gif":
                    return GifBack;

                case ".wav":
                    return WavBack;

                default:
                    return OtherBack;
            }
        }
    }
}