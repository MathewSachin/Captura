using Captura.Models;
using Captura.ViewModels;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace Captura
{
    public partial class MainWindow
    {        
        public MainWindow()
        {
            ServiceProvider.RegionProvider = new RegionSelector();

            ServiceProvider.MessageProvider = new MessageProvider();

            ServiceProvider.WebCamProvider = new WebCamProvider();
            
            InitializeComponent();

            ServiceProvider.MainWindow = new MainWindowProvider(this);

            if (App.CmdOptions.Tray)
                Hide();
            
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

            ServiceProvider.SystemTray = new SystemTray(SystemTray);
            
            Closing += (s, e) =>
            {
                if (!TryExit())
                    e.Cancel = true;
            };

            (DataContext as MainViewModel).Init(!App.CmdOptions.NoPersist, true, !App.CmdOptions.Reset, !App.CmdOptions.NoHotkeys);            
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

                WindowState = WindowState.Normal;

                Activate();
            }
        }

        bool TryExit()
        {
            var vm = DataContext as MainViewModel;

            if (vm.RecorderState == RecorderState.Recording)
            {
                if (!ServiceProvider.MessageProvider.ShowYesNo("A Recording is in progress. Are you sure you want to exit?", "Confirm Exit"))
                    return false;
            }

            vm.Dispose();
            SystemTray.Dispose();
            Application.Current.Shutdown();

            return true;
        }

        void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            TryExit();
        }
    }
}
