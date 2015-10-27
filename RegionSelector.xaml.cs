using System.Windows;
using System.Windows.Input;

namespace Captura
{
    public partial class RegionSelector : Window
    {
        public RegionSelector() { InitializeComponent(); }

        void DockPanel_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) { DragMove(); }
    }
}
