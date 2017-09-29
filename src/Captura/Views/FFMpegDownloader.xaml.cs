using System.Windows;

namespace Captura.Views
{
    public partial class FFMpegDownloader
    {
        public FFMpegDownloader()
        {
            InitializeComponent();
        }

        void CloseButton_Click(object Sender, RoutedEventArgs E) => Close();
    }
}
