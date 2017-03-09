using System.Windows;
using System.Windows.Input;

namespace Captura
{
    public partial class MainWindow
    {
        Point _moveOrigin;
        
        public MainWindow()
        {
            InitializeComponent();
            
            HotKeyManager.RegisterAll();
            
            Closed += (s, e) =>
            {
                HotKeyManager.Dispose();
                Application.Current.Shutdown();
            };
        }
        
        void Grid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();

            e.Handled = true;
        }

        void MinButton_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        void CloseButton_Click(object sender, RoutedEventArgs e) => Close();

        void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IsMouseCaptured)
                ReleaseMouseCapture();
        }

        void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _moveOrigin = e.GetPosition(this);
            CaptureMouse();
        }

        void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsMouseCaptured)
            {
                var offset = e.GetPosition(this) - _moveOrigin;
                Left += offset.X;
                Top += offset.Y;
            }
        }        
    }
}
