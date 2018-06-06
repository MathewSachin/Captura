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

        void OpenImageEditor(object Sender, RoutedEventArgs E)
        {
            new ImageEditorWindow().ShowAndFocus();
        }
    }
}
