using System.Windows;
using Captura.ViewModels;

namespace Captura
{
    public partial class VideoPage
    {
        public VideoPage()
        {
            InitializeComponent();
        }

        void OpenFFmpegLog(object Sender, RoutedEventArgs E)
        {
            FFmpegLogWindow.ShowInstance();
        }

        void OpenOverlayManager(object Sender, RoutedEventArgs E)
        {
            OverlayWindow.ShowInstance();
        }
    }
}
