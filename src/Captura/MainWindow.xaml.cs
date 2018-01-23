using System.Drawing;
using Captura.Models;
using Captura.ViewModels;
using Captura.Views;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using Screna;

namespace Captura
{
    public partial class MainWindow
    {
        public static MainWindow Instance { get; private set; }

        FFMpegDownloader _downloader;

        public MainWindow()
        {
            Instance = this;
            
            ServiceProvider.MessageProvider = new MessageProvider();
            
            FFMpegService.FFMpegDownloader += () =>
            {
                if (_downloader == null)
                {
                    _downloader = new FFMpegDownloader();
                    _downloader.Closed += (Sender, Args) => _downloader = null;
                }

                _downloader.ShowAndFocus();
            };
            
            InitializeComponent();
            
            if (App.CmdOptions.Tray)
                Hide();
            
            // Register for Windows Messages
            ComponentDispatcher.ThreadPreprocessMessage += (ref MSG Message, ref bool Handled) =>
            {
                const int wmHotkey = 786;

                if (Message.message == wmHotkey)
                {
                    var id = Message.wParam.ToInt32();

                    ServiceProvider.RaiseHotKeyPressed(id);
                }
            };
            
            Closing += (Sender, Args) =>
            {
                if (!TryExit())
                    Args.Cancel = true;
            };

            Loaded += (Sender, Args) =>
            {
                RepositionWindowIfOutside();

                if (DataContext is MainViewModel vm)
                {
                    vm.Init(!App.CmdOptions.NoPersist, true, !App.CmdOptions.Reset, !App.CmdOptions.NoHotkeys);
                }
            };
        }

        void RepositionWindowIfOutside()
        {
            var settings = ServiceProvider.Get<Settings>();

            var rect = new Rectangle(settings.UI.MainWindowLeft,
                settings.UI.MainWindowTop,
                (int) ActualWidth,
                (int) ActualHeight);

            // Convert as per DPI since WPF uses device independent units
            rect *= Dpi.Instance;
            
            if (!WindowProvider.DesktopRectangle.Contains(rect))
            {
                Left = 50;
                Top = 50;
            }
        }

        void Grid_PreviewMouseLeftButtonDown(object Sender, MouseButtonEventArgs Args)
        {
            DragMove();

            Args.Handled = true;
        }

        void MinButton_Click(object Sender, RoutedEventArgs Args) => SystemCommands.MinimizeWindow(this);

        void CloseButton_Click(object Sender, RoutedEventArgs Args) => Close();

        void SystemTray_TrayMouseDoubleClick(object Sender, RoutedEventArgs Args)
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
            if (DataContext is MainViewModel vm)
            {
                if (vm.RecorderState == RecorderState.Recording)
                {
                    if (!ServiceProvider.MessageProvider.ShowYesNo(
                        "A Recording is in progress. Are you sure you want to exit?", "Confirm Exit"))
                        return false;
                }

                vm.Dispose();
            }

            SystemTray.Dispose();

            return true;
        }

        void MenuExit_Click(object Sender, RoutedEventArgs Args) => Close();

        void HideButton_Click(object Sender, RoutedEventArgs Args) => Hide();
    }
}
