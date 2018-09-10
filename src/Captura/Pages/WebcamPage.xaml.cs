using System.Windows;

namespace Captura
{
    public partial class WebcamPage
    {
        public WebcamPage()
        {
            InitializeComponent();
        }

        void Preview_Click(object Sender, RoutedEventArgs E)
        {
            WebCamWindow.Instance.ShowAndFocus();
        }
    }
}
