using System.Windows;
using System.Windows.Input;

namespace Captura.View
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        void Window_PreviewMouseLeftButtonDown(object Sender, MouseButtonEventArgs E)
        {
            DragMove();
        }

        void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        void Settings_Click(object sender, RoutedEventArgs e)
        {
            new SettingsWindow().ShowDialog();
        }
    }
}
