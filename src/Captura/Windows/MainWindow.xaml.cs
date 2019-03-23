using System.Drawing;
using System.Linq;
using Captura.ViewModels;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace Captura
{
    public partial class MainWindow
    {
        public static MainWindow Instance { get; private set; }

        public MainWindow()
        {
            Instance = this;
            
            InitializeComponent();

            var mainModel = ServiceProvider.Get<MainModel>();

            mainModel.Init(!App.CmdOptions.NoPersist, !App.CmdOptions.Reset);

            var hotkeySetup = ServiceProvider.Get<HotkeySetup>();
            hotkeySetup.Setup();

            ServiceProvider.Get<TimerModel>().Init();

            Loaded += (Sender, Args) =>
            {
                RepositionWindowIfOutside();

                WebCamWindow.Instance.SetupWebcamPreview();

                hotkeySetup.ShowUnregistered();
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
            var rect = new RectangleF((float) Left,
                (float) Top,
                (float) ActualWidth,
                (float) ActualHeight).ApplyDpi();
            
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
            else this.ShowAndFocus();
        }

        bool TryExit()
        {
            var recordingVm = ServiceProvider.Get<RecordingViewModel>();

            if (!recordingVm.CanExit())
                return false;

            ServiceProvider.Dispose();

            return true;
        }

        void MenuExit_Click(object Sender, RoutedEventArgs Args) => Close();

        void HideButton_Click(object Sender, RoutedEventArgs Args) => Hide();

        void ShowMainWindow(object Sender, RoutedEventArgs E) => this.ShowAndFocus();
    }
}
