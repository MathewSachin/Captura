using System.Windows;

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
    }
}
