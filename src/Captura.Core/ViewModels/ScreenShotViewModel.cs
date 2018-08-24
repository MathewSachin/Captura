using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Captura.Models;
using Screna;

namespace Captura.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ScreenShotViewModel : ViewModelBase
    {
        readonly VideoViewModel _videoViewModel;
        readonly RecentViewModel _recentViewModel;
        readonly ISystemTray _systemTray;
        readonly IRegionProvider _regionProvider;
        readonly IMainWindow _mainWindow;

        public DiskWriter DiskWriter { get; }
        public ClipboardWriter ClipboardWriter { get; }
        public ImgurWriter ImgurWriter { get; }

        public ICommand SourceToggleCommand { get; } = new DelegateCommand(OnSourceToggleExecute);

        static void OnSourceToggleExecute(object M)
        {
            if (M is IImageWriterItem imageWriter)
            {
                imageWriter.Active = !imageWriter.Active;
            }
        }

        public ScreenShotViewModel(VideoViewModel VideoViewModel,
            RecentViewModel RecentViewModel,
            ISystemTray SystemTray,
            LanguageManager Loc,
            Settings Settings,
            IRegionProvider RegionProvider,
            IMainWindow MainWindow,
            DiskWriter DiskWriter,
            ClipboardWriter ClipboardWriter,
            ImgurWriter ImgurWriter) : base(Settings, Loc)
        {
            _videoViewModel = VideoViewModel;
            _recentViewModel = RecentViewModel;
            _systemTray = SystemTray;
            _regionProvider = RegionProvider;
            _mainWindow = MainWindow;
            this.DiskWriter = DiskWriter;
            this.ClipboardWriter = ClipboardWriter;
            this.ImgurWriter = ImgurWriter;

            ScreenShotCommand = new DelegateCommand(() => CaptureScreenShot());

            ScreenShotActiveCommand = new DelegateCommand(async () => await SaveScreenShot(ScreenShotWindow(Window.ForegroundWindow)));

            ScreenShotDesktopCommand = new DelegateCommand(async () => await SaveScreenShot(ScreenShotWindow(Window.DesktopWindow)));
        }

        public DelegateCommand ScreenShotCommand { get; }

        public DelegateCommand ScreenShotActiveCommand { get; }

        public DelegateCommand ScreenShotDesktopCommand { get; }

        public async Task SaveScreenShot(Bitmap Bmp, string FileName = null)
        {
            // Save to Disk or Clipboard
            if (Bmp != null)
            {
                var allTasks = _videoViewModel.AvailableImageWriters
                    .Where(M => M.Active)
                    .Select(M => M.Save(Bmp, SelectedScreenShotImageFormat, FileName, _recentViewModel));

                await Task.WhenAll(allTasks).ContinueWith(T => Bmp.Dispose());
            }
            else _systemTray.ShowTextNotification(Loc.ImgEmpty, null);
        }

        public Bitmap ScreenShotWindow(IWindow hWnd)
        {
            _systemTray.HideNotification();

            if (hWnd.Handle == Window.DesktopWindow.Handle)
            {
                return ScreenShot.Capture(Settings.IncludeCursor).Transform(Settings.ScreenShots);
            }

            var bmp = ScreenShot.CaptureTransparent(hWnd,
                Settings.IncludeCursor,
                Settings.ScreenShots.Resize,
                Settings.ScreenShots.ResizeWidth,
                Settings.ScreenShots.ResizeHeight);

            // Capture without Transparency
            if (bmp == null)
            {
                try
                {
                    return ScreenShot.Capture(hWnd, Settings.IncludeCursor)?.Transform(Settings.ScreenShots);
                }
                catch
                {
                    return null;
                }
            }

            return bmp.Transform(Settings.ScreenShots, true);
        }

        public async void CaptureScreenShot(string FileName = null)
        {
            _systemTray.HideNotification();

            var bmp = await GetScreenShot();

            await SaveScreenShot(bmp, FileName);
        }

        public async Task<Bitmap> GetScreenShot()
        {
            Bitmap bmp = null;

            var selectedVideoSource = _videoViewModel.SelectedVideoSourceKind?.Source;
            var includeCursor = Settings.IncludeCursor;

            switch (_videoViewModel.SelectedVideoSourceKind)
            {
                case WindowSourceProvider _:
                    IWindow hWnd = Window.DesktopWindow;

                    switch (selectedVideoSource)
                    {
                        case WindowItem windowItem:
                            hWnd = windowItem.Window;
                            break;
                    }

                    bmp = ScreenShotWindow(hWnd);
                    break;

                case DeskDuplSourceProvider _:
                    if (selectedVideoSource is DeskDuplItem deskDuplItem)
                    {
                        bmp = ScreenShot.Capture(deskDuplItem.Rectangle, includeCursor);
                    }
                    break;

                case FullScreenSourceProvider _:
                    var hide = _mainWindow.IsVisible && Settings.UI.HideOnFullScreenShot;

                    if (hide)
                    {
                        _mainWindow.IsVisible = false;

                        // Ensure that the Window is hidden
                        await Task.Delay(300);
                    }

                    bmp = ScreenShot.Capture(includeCursor);

                    if (hide)
                        _mainWindow.IsVisible = true;
                    break;

                case ScreenSourceProvider _:
                    if (selectedVideoSource is ScreenItem screen)
                        bmp = screen.Capture(includeCursor);

                    bmp = bmp?.Transform(Settings.ScreenShots);
                    break;

                case RegionSourceProvider _:
                    bmp = ScreenShot.Capture(_regionProvider.SelectedRegion, includeCursor);
                    bmp = bmp.Transform(Settings.ScreenShots);
                    break;
            }

            return bmp;
        }

        public IEnumerable<ImageFormat> ScreenShotImageFormats { get; } = new[]
        {
            ImageFormat.Png,
            ImageFormat.Jpeg,
            ImageFormat.Bmp,
            ImageFormat.Tiff,
            ImageFormat.Wmf,
            ImageFormat.Exif,
            ImageFormat.Gif,
            ImageFormat.Icon,
            ImageFormat.Emf
        };

        ImageFormat _screenShotImageFormat = ImageFormat.Png;

        public ImageFormat SelectedScreenShotImageFormat
        {
            get => _screenShotImageFormat;
            set
            {
                if (_screenShotImageFormat == value)
                    return;

                _screenShotImageFormat = value;

                OnPropertyChanged();
            }
        }
    }
}