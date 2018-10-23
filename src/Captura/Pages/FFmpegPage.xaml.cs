using System.Windows;
using System.Windows.Input;
using Captura.ViewModels;

namespace Captura
{
    public partial class FFmpegPage
    {
        void OpenFFmpegLog(object Sender, RoutedEventArgs E)
        {
            FFmpegLogWindow.ShowInstance();
        }
        
        void FFmpegDownload(object Sender, RoutedEventArgs E)
        {
            FFmpegService.FFmpegDownloader?.Invoke();
        }

        void ConfigCodecs(object Sender, RoutedEventArgs E)
        {
            FFmpegCodecWindow.ShowInstance();
        }

        void SelectFFmpegFolder(object Sender, MouseButtonEventArgs E)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.SelectFFmpegFolderCommand.ExecuteIfCan();
            }
        }
    }
}
