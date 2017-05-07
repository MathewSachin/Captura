using Captura.Models;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace Captura
{
    public partial class MainWindow
    {
        Point _moveOrigin;
        
        public MainWindow()
        {
            ServiceProvider.Register<Action>(ServiceName.Focus, () =>
            {
                Show();
                WindowState = WindowState.Normal;
                Focus();
            });

            ServiceProvider.Register<Action>(ServiceName.Exit, () =>
            {
                (DataContext as MainViewModel).Dispose();
                Application.Current.Shutdown();
            });

            ServiceProvider.Register<IVideoItem>(ServiceName.RegionSource, RegionItem.Instance);

            ServiceProvider.Register<Action<bool>>(ServiceName.RegionSelectorVisibility, visible =>
            {
                if (visible)
                    RegionSelector.Instance.Show();
                else RegionSelector.Instance.Hide();
            });

            // Register for Windows Messages
            ComponentDispatcher.ThreadPreprocessMessage += (ref MSG Message, ref bool Handled) =>
            {
                const int WmHotkey = 786;

                if (Message.message == WmHotkey)
                {
                    var id = Message.wParam.ToInt32();

                    ServiceProvider.RaiseHotKeyPressed(id);
                }
            };

            InitializeComponent();
            
            Closed += (s, e) =>
            {
                ServiceProvider.Get<Action>(ServiceName.Exit).Invoke();
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
