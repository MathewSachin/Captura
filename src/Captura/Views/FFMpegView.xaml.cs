using Captura.Views;
using System.Windows;

namespace Captura
{
    public partial class FFMpegView
    {
        void OpenFFMpegLog(object sender, RoutedEventArgs e)
        {
            FFMpegLogView.Instance.ShowAndFocus();
        }

        FFMpegDownloader _downloader;

        void FFDownload(object sender, RoutedEventArgs e)
        {
            if (_downloader == null)
            {
                _downloader = new FFMpegDownloader();
                _downloader.Closed += (s, args) => _downloader = null;
            }

            _downloader.ShowAndFocus();
        }
    }
}
