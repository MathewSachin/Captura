using System.Windows;

namespace Captura
{
    public partial class HotkeyView
    {
        public HotkeyView()
        {
            InitializeComponent();
        }

        void Reset_Click(object sender, RoutedEventArgs e)
        {
            HotKeyManager.Reset();
        }
    }
}
