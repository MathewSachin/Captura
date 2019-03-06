using System.Windows;
using System.Windows.Input;
using Captura.ViewModels;
using Captura.Views;

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
            FFmpegDownloaderWindow.ShowInstance();
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
