﻿using System.Collections.Generic;
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
        readonly IVideoSourcePicker _sourcePicker;
        readonly IAudioPlayer _audioPlayer;

        public DiskWriter DiskWriter { get; }
        public ClipboardWriter ClipboardWriter { get; }
        public ImageUploadWriter ImgurWriter { get; }

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
            ImageUploadWriter ImgurWriter,
            IVideoSourcePicker SourcePicker,
            IAudioPlayer AudioPlayer) : base(Settings, Loc)
        {
            _videoViewModel = VideoViewModel;
            _recentViewModel = RecentViewModel;
            _systemTray = SystemTray;
            _regionProvider = RegionProvider;
            _mainWindow = MainWindow;
            _sourcePicker = SourcePicker;
            _audioPlayer = AudioPlayer;
            this.DiskWriter = DiskWriter;
            this.ClipboardWriter = ClipboardWriter;
            this.ImgurWriter = ImgurWriter;

            ScreenShotCommand = new DelegateCommand(() => CaptureScreenShot());

            ScreenShotActiveCommand = new DelegateCommand(async () => await SaveScreenShot(ScreenShotWindow(Window.ForegroundWindow)));

            ScreenShotDesktopCommand = new DelegateCommand(async () => await SaveScreenShot(ScreenShotWindow(Window.DesktopWindow)));

            ScreenshotRegionCommand = new DelegateCommand(async () => await ScreenshotRegion());

            ScreenshotWindowCommand = new DelegateCommand(async () => await ScreenshotWindow());

            ScreenshotScreenCommand = new DelegateCommand(async () => await ScreenshotScreen());
        }

        public DelegateCommand ScreenShotCommand { get; }

        public DelegateCommand ScreenShotActiveCommand { get; }

        public DelegateCommand ScreenShotDesktopCommand { get; }

        public DelegateCommand ScreenshotRegionCommand { get; }

        public DelegateCommand ScreenshotWindowCommand { get; }

        public DelegateCommand ScreenshotScreenCommand { get; }

        async Task ScreenshotRegion()
        {
            var region = _sourcePicker.PickRegion();

            if (region == null)
                return;

            await SaveScreenShot(ScreenShot.Capture(region.Value));
        }

        async Task ScreenshotWindow()
        {
            var window = _sourcePicker.PickWindow();

            if (window != null)
            {
                var img = ScreenShot.Capture(window);

                await SaveScreenShot(img);
            }
        }

        async Task ScreenshotScreen()
        {
            var screen = _sourcePicker.PickScreen();

            if (screen != null)
            {
                var img = ScreenShot.Capture(screen);

                await SaveScreenShot(img);
            }
        }

        public async Task SaveScreenShot(Bitmap Bmp, string FileName = null)
        {
            _audioPlayer.Play(SoundKind.Shot);

            if (Bmp != null)
            {
                var allTasks = _videoViewModel.AvailableImageWriters
                    .Where(M => M.Active)
                    .Select(M => M.Save(Bmp, SelectedScreenShotImageFormat, FileName));

                await Task.WhenAll(allTasks).ContinueWith(T => Bmp.Dispose());
            }
            else _systemTray.ShowNotification(new TextNotification(Loc.ImgEmpty));
        }

        public Bitmap ScreenShotWindow(IWindow Window)
        {
            _systemTray.HideNotification();

            if (Window.Handle == Screna.Window.DesktopWindow.Handle)
            {
                return ScreenShot.Capture(Settings.IncludeCursor);
            }

            try
            {
                Bitmap bmp = null;

                if (Settings.ScreenShots.WindowShotTransparent)
                {
                    bmp = ScreenShot.CaptureTransparent(Window, Settings.IncludeCursor);
                }

                // Capture without Transparency
                return bmp ?? ScreenShot.Capture(Window, Settings.IncludeCursor);
            }
            catch
            {
                return null;
            }
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
                    break;

                case RegionSourceProvider _:
                    bmp = ScreenShot.Capture(_regionProvider.SelectedRegion, includeCursor);
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
                if (Equals(_screenShotImageFormat, value))
                    return;

                _screenShotImageFormat = value;

                OnPropertyChanged();
            }
        }
    }
}