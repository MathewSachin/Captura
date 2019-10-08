using System.Windows;

namespace Captura
{
    public partial class CollapsedBar
    {
        public CollapsedBar()
        {
            InitializeComponent();
        }

        void OpenSettings(object Sender, RoutedEventArgs E)
        {
            SettingsWindow.ShowInstance();
        }
    }
}
