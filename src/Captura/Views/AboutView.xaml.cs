using System.Windows;
using Captura.Views;

namespace Captura
{
    public partial class AboutView
    {
        void ViewLicenses(object Sender, RoutedEventArgs E)
        {
            LicensesWindow.ShowInstance();
        }

        void Translate(object Sender, RoutedEventArgs E)
        {
            TranslationWindow.ShowInstance();
        }

        void ViewCrashLogs(object Sender, RoutedEventArgs E)
        {
            CrashLogsWindow.ShowInstance();
        }
    }
}
