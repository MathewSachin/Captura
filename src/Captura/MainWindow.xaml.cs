using Captura.Models;
using Captura.ViewModels;
using System;
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
            ServiceProvider.Register<IRegionProvider>(ServiceName.RegionProvider, new RegionProvider());
            
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

            ServiceProvider.Register<ISystemTray>(ServiceName.SystemTray, new SystemTray(SystemTray));

            Closed += (s, e) => MenuExit_Click();
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

        void SystemTray_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Hide();
            }
            else
            {
                Show();
                Focus();
            }
        }

        void MenuExit_Click(object sender = null, RoutedEventArgs e = null)
        {
            (DataContext as MainViewModel).Dispose();
            SystemTray.Dispose();
            Application.Current.Shutdown();
        }
    }
}
