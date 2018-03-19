using System.Windows;

namespace Captura
{
    public partial class FFMpegView
    {
        void OpenFFMpegLog(object Sender, RoutedEventArgs E)
        {
            FFMpegLogView.Instance.ShowAndFocus();
        }
        
        void FFDownload(object Sender, RoutedEventArgs E)
        {
            FFMpegService.FFMpegDownloader?.Invoke();
        }

        void ConfigCodecs(object Sender, RoutedEventArgs E)
        {
            new FFMpegCodecWindow().ShowAndFocus();
        }
    }
}
