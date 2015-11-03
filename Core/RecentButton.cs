using System.Diagnostics;
using System.IO;
using System.Windows.Media;
using System.Windows.Input;

namespace Captura
{
    public class RecentButton : Fluent.Button
    {
        static readonly SolidColorBrush AviBack = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B7000000")),
            GifBack = new SolidColorBrush(Colors.DarkGreen),
            WavBack = new SolidColorBrush(Colors.DarkOrange),
            OtherBack = new SolidColorBrush(Colors.DarkViolet);

        public RoutedUICommand RecentButtonClick = new RoutedUICommand();

        public RecentButton(string FilePath)
        {
            Header = Path.GetFileName(FilePath);
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