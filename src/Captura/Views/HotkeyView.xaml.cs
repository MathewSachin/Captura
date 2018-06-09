using System.Windows;
using Captura.Views;

namespace Captura
{
    public partial class HotkeyView
    {
        public HotkeyView()
        {
            InitializeComponent();
        }

        void OpenHotkeyManager(object Sender, RoutedEventArgs E)
        {
            new HotkeysWindow().ShowAndFocus();
        }
    }
}
