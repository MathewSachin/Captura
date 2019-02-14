using System.Drawing;
using System.Linq;
using Captura.Models;
using Captura.ViewModels;
using Captura.Views;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;

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

                SetupWebcamPreview();

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

        void SetupWebcamPreview()
        {
            var webcamModel = ServiceProvider.Get<WebcamModel>();
            var camControl = WebCamWindow.Instance.GetWebCamControl();

            // Open Preview Window
            webcamModel.PreviewClicked += () => WebCamWindow.Instance.ShowAndFocus();

            camControl.IsVisibleChanged += (S, E) => SwitchWebcamPreview();

            void OnSizeChange()
            {
                var rect = GetPreviewWindowRect();

                webcamModel.WebcamCapture?.UpdatePreview(null, rect);
            }

            camControl.SizeChanged += (S, E) => OnSizeChange();

            webcamModel.PropertyChanged += (S, E) =>
            {
                if (E.PropertyName == nameof(WebcamModel.SelectedCam) && webcamModel.WebcamCapture != null)
                {
                    SwitchWebcamPreview();
                }
            };
        }

        Rectangle GetPreviewWindowRect()
        {
            var camControl = WebCamWindow.Instance.GetWebCamControl();

            var rect = new RectangleF(5, 40, (float)camControl.ActualWidth, (float)camControl.ActualHeight);

            return ApplyDpi(rect);
        }

        void SwitchWebcamPreview()
        {
            var webcamModel = ServiceProvider.Get<WebcamModel>();
            var camControl = WebCamWindow.Instance.GetWebCamControl();

            if (webcamModel.WebcamCapture == null)
                return;

            var platformServices = ServiceProvider.Get<IPlatformServices>();

            if (camControl.IsVisible)
            {
                if (PresentationSource.FromVisual(camControl) is HwndSource source)
                {
                    var win = platformServices.GetWindow(source.Handle);

                    var rect = GetPreviewWindowRect();

                    webcamModel.WebcamCapture.UpdatePreview(win, rect);
                }
            }
            else if (PresentationSource.FromVisual(this) is HwndSource source)
            {
                var win = platformServices.GetWindow(source.Handle);

                var rect = ApplyDpi(new Rectangle(280, 1, 50, 40));

                webcamModel.WebcamCapture.UpdatePreview(win, rect);
            }
        }

        Rectangle ApplyDpi(RectangleF Rectangle)
        {
            return new Rectangle((int)(Rectangle.Left * Dpi.X),
                (int)(Rectangle.Top * Dpi.Y),
                (int)(Rectangle.Width * Dpi.X),
                (int)(Rectangle.Height * Dpi.Y));
        }

        void RepositionWindowIfOutside()
        {
            // Window dimensions taking care of DPI
            var rect = ApplyDpi(new RectangleF((float) Left,
                (float) Top,
                (float) ActualWidth,
                (float) ActualHeight));
            
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
