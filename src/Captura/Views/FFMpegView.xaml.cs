using System.Windows;

namespace Captura
{
    public partial class FFmpegView
    {
        void OpenFFmpegLog(object Sender, RoutedEventArgs E)
        {
            FFmpegLogView.Instance.ShowAndFocus();
        }
        
        void FFmpegDownload(object Sender, RoutedEventArgs E)
        {
            FFmpegService.FFmpegDownloader?.Invoke();
        }

        void ConfigCodecs(object Sender, RoutedEventArgs E)
        {
            new FFmpegCodecWindow().ShowAndFocus();
        }
    }
}
