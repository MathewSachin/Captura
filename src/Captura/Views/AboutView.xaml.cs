using System.Windows;

namespace Captura
{
    public partial class AboutView
    {
        void OpenFFMpegLog(object sender, RoutedEventArgs e)
        {
            FFMpegLogView.Instance.ShowAndFocus();
        }
    }
}
