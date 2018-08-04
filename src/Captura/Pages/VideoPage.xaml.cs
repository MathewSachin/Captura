using System.Windows;

namespace Captura
{
    public partial class VideoPage
    {
        public VideoPage()
        {
            InitializeComponent();
        }

        void Preview_Click(object Sender, RoutedEventArgs E)
        {
            WebCamWindow.Instance.ShowAndFocus();
        }
    }
}
