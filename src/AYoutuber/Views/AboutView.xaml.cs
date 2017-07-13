using System.Windows;

namespace Captura
{
    public partial class AboutView
    {
        void OpenExtras(object sender, RoutedEventArgs e)
        {
            MoreOptionsWindow.Instance.ShowAndFocus();
        }

        void OpenFFMpegLog(object sender, RoutedEventArgs e)
        {
            FFMpegLogView.Instance.ShowAndFocus();
        }
    }
}
