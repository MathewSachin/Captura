using System.Windows;
using Captura.ViewModels;

namespace Captura
{
    public partial class WebcamPage
    {
        public WebcamPage()
        {
            InitializeComponent();

            ServiceProvider.Get<MainViewModel>().Refreshed += () =>
            {
                WebcamComboBox.Shake();
            };
        }

        void Preview_Click(object Sender, RoutedEventArgs E)
        {
            WebCamWindow.Instance.ShowAndFocus();
        }
    }
}
