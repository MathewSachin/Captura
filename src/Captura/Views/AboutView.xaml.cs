using System.Windows;
using Captura.Views;

namespace Captura
{
    public partial class AboutView
    {
        void ViewLicenses(object Sender, RoutedEventArgs E)
        {
            new LicensesWindow().ShowAndFocus();
        }

        void Translate(object Sender, RoutedEventArgs E)
        {
            new TranslationWindow().ShowAndFocus();
        }

        void ViewCrashLogs(object Sender, RoutedEventArgs E)
        {
            new CrashLogsWindow().ShowAndFocus();
        }
    }
}
