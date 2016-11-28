using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace Captura
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            
            Closed += (s, e) => Application.Current.Shutdown();
        }
        
        void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e) => Process.Start(e.Uri.AbsoluteUri);
    }
}
