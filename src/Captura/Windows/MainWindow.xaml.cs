using System.Drawing;
using System.Linq;
using Captura.Models;
using Captura.ViewModels;
using Captura.Views;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace Captura
{
    public partial class MainWindow
    {
        public static MainWindow Instance { get; private set; }

        FFmpegDownloaderWindow _downloader;

        public MainWindow()
        {
            Instance = this;
            
            FFmpegService.FFmpegDownloader += () =>
            {
                if (_downloader == null)
                {
                    _downloader = new FFmpegDownloaderWindow();
                    _downloader.Closed += (Sender, Args) => _downloader = null;
                }

                _downloader.ShowAndFocus();
            };
            
            InitializeComponent();

            var mainModel = ServiceProvider.Get<MainModel>();

            mainModel.Init(!App.CmdOptions.NoPersist, !App.CmdOptions.Reset, !App.CmdOptions.NoHotkeys);
            ServiceProvider.Get<HotkeyActionRegisterer>().Register();
            ServiceProvider.Get<TimerModel>().Init();

            var listener = new HotkeyListener();

            var hotkeyManager = ServiceProvider.Get<HotKeyManager>();

            listener.HotkeyReceived += Id => hotkeyManager.ProcessHotkey(Id);

            ServiceProvider.Get<HotKeyManager>().HotkeyPressed += Service =>
            {
                switch (Service)
                {
                    case ServiceName.OpenImageEditor:
                        new ImageEditorWindow().ShowAndFocus();
                        break;

                    case ServiceName.ShowMainWindow:
                        this.ShowAndFocus();
                        break;
                }
            };

            Loaded += (Sender, Args) =>
            {
                RepositionWindowIfOutside();

                mainModel.ViewLoaded();
            };

            if (App.CmdOptions.Tray || ServiceProvider.Get<Settings>().Tray.MinToTrayOnStartup)
                Hide();

            Closing += (Sender, Args) =>
            {
                if (!TryExit())
                    Args.Cancel = true;
            };
        }

        void RepositionWindowIfOutside()
        {
            // Window dimensions taking care of DPI
            var rect = new Rectangle((int)(Left * Dpi.X),
                (int)(Top * Dpi.Y),
                (int)(ActualWidth * Dpi.X),
                (int)(ActualHeight * Dpi.Y));
            
            if (!Screen.AllScreens.Any(M => M.Bounds.Contains(rect)))
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

        void CloseButton_Click(object Sender, RoutedEventArgs Args)
        {
            if (ServiceProvider.Get<Settings>().Tray.MinToTrayOnClose)
            {
                Hide();
            }
            else Close();
        }

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
            var recordingModel = ServiceProvider.Get<RecordingModel>();

            if (!recordingModel.CanExit())
                return false;

            ServiceProvider.Dispose();

            SystemTray.Dispose();

            return true;
        }

        void MenuExit_Click(object Sender, RoutedEventArgs Args) => Close();

        void HideButton_Click(object Sender, RoutedEventArgs Args) => Hide();

        void ShowMainWindow(object Sender, RoutedEventArgs E)
        {
            this.ShowAndFocus();
        }
    }
}
