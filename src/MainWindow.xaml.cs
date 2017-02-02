using System.Windows;
using System.Windows.Input;

namespace Captura
{
    public partial class MainWindow
    {
        ConfigWindow _configWindow;

        public MainWindow()
        {
            InitializeComponent();

            _configWindow = new ConfigWindow();
            _configWindow.Closing += (s, e) =>
            {
                _configWindow.Hide();
                e.Cancel = true;
            };
            
            Closed += (s, e) => Application.Current.Shutdown();
        }

        void ConfigButton_Click(object sender, RoutedEventArgs e)
        {
            _configWindow.Show();
            _configWindow.Focus();
        }

        void Grid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();

        void MinButton_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        void CloseButton_Click(object sender, RoutedEventArgs e) => Close();
    }
}
