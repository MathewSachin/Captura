using System.Windows;
using System.Windows.Input;
using Keys = System.Windows.Forms.Keys;

namespace Captura
{
    public partial class MainWindow
    {
        ConfigWindow _configWindow;
        HotKey _recordStopHotkey;

        public MainWindow()
        {
            InitializeComponent();

            _configWindow = new ConfigWindow();
            _configWindow.Closing += (s, e) =>
            {
                _configWindow.Hide();
                e.Cancel = true;
            };

            _recordStopHotkey = new HotKey(this, Keys.R, HotKey.Alt | HotKey.Ctrl | HotKey.Shift);
            _recordStopHotkey.Triggered += () =>
            {
                var command = App.MainViewModel.RecordCommand;

                if (command.CanExecute(null))
                    command.Execute(null);
            };
            
            Closed += (s, e) =>
            {
                _recordStopHotkey.Dispose();
                Application.Current.Shutdown();
            };
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
