using Captura.Models;
using Captura.ViewModels;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using DPoint = System.Drawing.Point;

namespace Captura
{
    public partial class MainWindow
    {        
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

            ServiceProvider.Register<Func<Rectangle>>(ServiceName.RegionRectangle, () => RegionSelector.Instance.Rectangle);

            ServiceProvider.Register<Action<Rectangle>>(ServiceName.SetRegionRectangle, rect => RegionSelector.Instance.Rectangle = rect);

            ServiceProvider.Register<Action<bool>>(ServiceName.Minimize, minimize =>
            {
                WindowState = minimize ? WindowState.Minimized : WindowState.Normal;
            });

            ServiceProvider.Register<Func<DPoint>>(ServiceName.MainWindowLocation, () => new DPoint((int) Left, (int) Top));

            ServiceProvider.Register<Action<DPoint>>(ServiceName.SetMainWindowLocation, point =>
            {
                Left = point.X;
                Top = point.Y;
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

        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);

            if (WindowState == WindowState.Minimized && Settings.Instance.MinimizeToTray)
                Hide();
        }

        void Grid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();

            e.Handled = true;
        }

        void MinButton_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        void CloseButton_Click(object sender, RoutedEventArgs e) => Close();
    }
}
