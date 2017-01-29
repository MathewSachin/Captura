using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Captura
{
    public partial class AboutView : Page
    {
        void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }
    }
}
