using System.Windows;
using Captura.ViewModels;

namespace Captura
{
    public partial class VideoEncoderPage
    {
        public VideoEncoderPage()
        {
            InitializeComponent();

            if (DataContext is MainViewModel vm)
            {
                vm.Refreshed += () => VideoWriterComboBox.Shake();
            }
        }

        void OpenFFmpegLog(object Sender, RoutedEventArgs E)
        {
            FFmpegLogWindow.ShowInstance();
        }
    }
}
