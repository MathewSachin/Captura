using Captura.Models;
using Screna;
using Screna.Audio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Win32;
using Timer = System.Timers.Timer;
using Window = Screna.Window;

namespace Captura.ViewModels
{
    public partial class MainViewModel : ViewModelBase, IDisposable
    {
        #region Fields
        Timer _timer;
        readonly Timing _timing = new Timing();
        IRecorder _recorder;
        string _currentFileName;
        bool _isVideo;
        public static readonly RectangleConverter RectangleConverter = new RectangleConverter();
        readonly SynchronizationContext _syncContext = SynchronizationContext.Current;
        
        readonly ISystemTray _systemTray;
        readonly IRegionProvider _regionProvider;
        readonly WebcamOverlay _webcamOverlay;
        readonly IMainWindow _mainWindow;
        #endregion
        
        public MainViewModel(AudioSource AudioSource,
            VideoViewModel VideoViewModel,
            ISystemTray SystemTray,
            IRegionProvider RegionProvider,
            IWebCamProvider WebCamProvider,
            WebcamOverlay WebcamOverlay,
            IMainWindow MainWindow,
            Settings Settings,
            RecentViewModel RecentViewModel,
            LanguageManager LanguageManager,
            HotKeyManager HotKeyManager,
            CustomOverlaysViewModel CustomOverlays,
            CustomImageOverlaysViewModel CustomImageOverlays) : base(Settings, LanguageManager)
        {
            this.AudioSource = AudioSource;
            this.VideoViewModel = VideoViewModel;
            _systemTray = SystemTray;
            _regionProvider = RegionProvider;
            this.WebCamProvider = WebCamProvider;
            _webcamOverlay = WebcamOverlay;
            _mainWindow = MainWindow;
            this.RecentViewModel = RecentViewModel;
            this.HotKeyManager = HotKeyManager;
            this.CustomOverlays = CustomOverlays;
            this.CustomImageOverlays = CustomImageOverlays;

            #region Commands
            ScreenShotCommand = new DelegateCommand(() => CaptureScreenShot());
            
            ScreenShotActiveCommand = new DelegateCommand(() => SaveScreenShot(ScreenShotWindow(Window.ForegroundWindow)));

            ScreenShotDesktopCommand = new DelegateCommand(() => SaveScreenShot(ScreenShotWindow(Window.DesktopWindow)));
            
            RecordCommand = new DelegateCommand(OnRecordExecute);
            
            RefreshCommand = new DelegateCommand(OnRefresh);
            
            PauseCommand = new DelegateCommand(OnPauseExecute, false);

            OpenOutputFolderCommand = new DelegateCommand(OpenOutputFolder);

            SelectOutputFolderCommand = new DelegateCommand(SelectOutputFolder);

            ResetFFMpegFolderCommand = new DelegateCommand(() => Settings.FFMpeg.FolderPath = "");
            #endregion

            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;

            AudioSource.PropertyChanged += (Sender, Args) =>
            {
                switch (Args.PropertyName)
                {
                    case nameof(AudioSource.SelectedRecordingSource):
                    case nameof(AudioSource.SelectedLoopbackSource):
                    case null:
                    case "":
                        CheckFunctionalityAvailability();
                        break;
                }
            };

            VideoViewModel.PropertyChanged += (Sender, Args) =>
            {
                switch (Args.PropertyName)
                {
                    case nameof(VideoViewModel.SelectedVideoSourceKind):
                    case nameof(VideoViewModel.SelectedVideoSource):
                    case null:
                    case "":
                        CheckFunctionalityAvailability();
                        break;
                }
            };

            // If Output Dircetory is not set. Set it to Documents\Captura\
            if (string.IsNullOrWhiteSpace(Settings.OutPath))
                Settings.OutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Captura");

            // Create the Output Directory if it does not exist
            Settings.EnsureOutPath();

            // Handle Hoykeys
            HotKeyManager.HotkeyPressed += Service =>
            {
                switch (Service)
                {
                    case ServiceName.Recording:
                        RecordCommand?.ExecuteIfCan();
                        break;

                    case ServiceName.Pause:
                        PauseCommand?.ExecuteIfCan();
                        break;

                    case ServiceName.ScreenShot:
                        ScreenShotCommand?.ExecuteIfCan();
                        break;

                    case ServiceName.ActiveScreenShot:
                        ScreenShotActiveCommand?.ExecuteIfCan();
                        break;

                    case ServiceName.DesktopScreenShot:
                        ScreenShotDesktopCommand?.ExecuteIfCan();
                        break;

                    case ServiceName.ToggleMouseClicks:
                        Settings.Clicks.Display = !Settings.Clicks.Display;
                        break;

                    case ServiceName.ToggleKeystrokes:
                        Settings.Keystrokes.Display = !Settings.Keystrokes.Display;
                        break;
                }
            };
        }

        async void OnRecordExecute()
        {
            if (RecorderState == RecorderState.NotRecording)
                StartRecording();
            else await StopRecording();
        }

        void OnRefresh()
        {
            WindowProvider.RefreshDesktopSize();

            VideoViewModel.RefreshVideoSources();

            VideoViewModel.RefreshCodecs();

            AudioSource.Refresh();

            WebCamProvider.Refresh();

            Status.LocalizationKey = nameof(LanguageManager.Refreshed);
        }

        void SystemEvents_PowerModeChanged(object Sender, PowerModeChangedEventArgs E)
        {
            if (E.Mode == PowerModes.Suspend && RecorderState == RecorderState.Recording)
            {
                OnPauseExecute();
            }
        }

        void OnPauseExecute()
        {
            if (RecorderState == RecorderState.Paused)
            {
                _systemTray.HideNotification();

                _recorder.Start();
                _timing?.Start();
                _timer?.Start();

                RecorderState = RecorderState.Recording;
                Status.LocalizationKey = nameof(LanguageManager.Recording);
            }
            else
            {
                _recorder.Stop();
                _timer?.Stop();
                _timing?.Pause();

                RecorderState = RecorderState.Paused;
                Status.LocalizationKey = nameof(LanguageManager.Paused);

                _systemTray.ShowTextNotification(Loc.Paused, 3000, null);
            }
        }

        void RestoreRemembered()
        {
            // Restore Video Source
            if (!string.IsNullOrEmpty(Settings.Video.SourceKind))
            {
                var kind = VideoViewModel.AvailableVideoSourceKinds.FirstOrDefault(M => M.Name == Settings.Video.SourceKind);

                if (kind != null)
                {
                    VideoViewModel.SelectedVideoSourceKind = kind;

                    switch (kind)
                    {
                        case RegionSourceProvider _:
                            if (RectangleConverter.ConvertFromInvariantString(Settings.Video.Source) is Rectangle rect)
                                _regionProvider.SelectedRegion = rect;
                            break;

                        default:
                            var source = VideoViewModel.AvailableVideoSources
                                .FirstOrDefault(S => S.ToString() == Settings.Video.Source);

                            if (source != null)
                                VideoViewModel.SelectedVideoSource = source;
                            break;
                    }
                }
            }

            // Restore Video Codec
            if (!string.IsNullOrEmpty(Settings.Video.WriterKind))
            {
                var kind = VideoViewModel.AvailableVideoWriterKinds.FirstOrDefault(W => W.Name == Settings.Video.WriterKind);

                if (kind != null)
                {
                    VideoViewModel.SelectedVideoWriterKind = kind;

                    var codec = VideoViewModel.AvailableVideoWriters.FirstOrDefault(C => C.ToString() == Settings.Video.Writer);

                    if (codec != null)
                        VideoViewModel.SelectedVideoWriter = codec;
                }
            }
            
            // Restore Microphone
            if (!string.IsNullOrEmpty(Settings.Audio.Microphone))
            {
                var source = AudioSource.AvailableRecordingSources.FirstOrDefault(C => C.ToString() == Settings.Audio.Microphone);

                if (source != null)
                    AudioSource.SelectedRecordingSource = source;
            }

            // Restore Loopback Speaker
            if (!string.IsNullOrEmpty(Settings.Audio.Speaker))
            {
                var source = AudioSource.AvailableLoopbackSources.FirstOrDefault(C => C.ToString() == Settings.Audio.Speaker);

                if (source != null)
                    AudioSource.SelectedLoopbackSource = source;
            }

            // Restore ScreenShot Format
            if (!string.IsNullOrEmpty(Settings.ScreenShots.ImageFormat))
            {
                var format = ScreenShotImageFormats.FirstOrDefault(F => F.ToString() == Settings.ScreenShots.ImageFormat);

                if (format != null)
                    SelectedScreenShotImageFormat = format;
            }

            // Restore ScreenShot Target
            if (!string.IsNullOrEmpty(Settings.ScreenShots.SaveTarget))
            {
                var saveTo = VideoViewModel.AvailableImageWriters.FirstOrDefault(S => S.ToString() == Settings.ScreenShots.SaveTarget);

                if (saveTo != null)
                    VideoViewModel.SelectedImageWriter = saveTo;
            }

            // Restore Webcam
            if (!string.IsNullOrEmpty(Settings.Video.Webcam))
            {
                var webcam = WebCamProvider.AvailableCams.FirstOrDefault(C => C.Name == Settings.Video.Webcam);

                if (webcam != null)
                {
                    WebCamProvider.SelectedCam = webcam;
                }
            }
        }

        bool _persist, _hotkeys;

        public void Init(bool Persist, bool Timer, bool Remembered, bool Hotkeys)
        {
            _persist = Persist;
            _hotkeys = Hotkeys;

            if (Timer)
            {
                _timer = new Timer(500);
                _timer.Elapsed += TimerOnElapsed;
            }

            // Register Hotkeys if not console
            if (_hotkeys)
                HotKeyManager.RegisterAll();

            VideoViewModel.Init();

            if (Remembered)
                RestoreRemembered();
        }

        void Remember()
        {
            // Remember Video Source
            Settings.Video.SourceKind = VideoViewModel.SelectedVideoSourceKind.Name;

            switch (VideoViewModel.SelectedVideoSourceKind)
            {
                case RegionSourceProvider _:
                    var rect = _regionProvider.SelectedRegion;
                    Settings.Video.Source = RectangleConverter.ConvertToInvariantString(rect);
                    break;

                default:
                    Settings.Video.Source = VideoViewModel.SelectedVideoSource.ToString();
                    break;
            }

            // Remember Video Codec
            Settings.Video.WriterKind = VideoViewModel.SelectedVideoWriterKind.Name;
            Settings.Video.Writer = VideoViewModel.SelectedVideoWriter.ToString();

            // Remember Audio Sources
            Settings.Audio.Microphone = AudioSource.SelectedRecordingSource.ToString();
            Settings.Audio.Speaker = AudioSource.SelectedLoopbackSource.ToString();
            
            // Remember ScreenShot Format
            Settings.ScreenShots.ImageFormat = SelectedScreenShotImageFormat.ToString();

            // Remember ScreenShot Target
            Settings.ScreenShots.SaveTarget = VideoViewModel.SelectedImageWriter.ToString();

            // Remember Webcam
            Settings.Video.Webcam = WebCamProvider.SelectedCam.Name;
        }
        
        public void Dispose()
        {
            SystemEvents.PowerModeChanged -= SystemEvents_PowerModeChanged;

            if (_hotkeys)
                HotKeyManager.Dispose();

            AudioSource.Dispose();

            RecentViewModel.Dispose();
            
            // Remember things if not console.
            if (_persist)
            {
                CustomOverlays.Dispose();
                CustomImageOverlays.Dispose();

                Remember();

                Settings.Save();
            }
        }
        
        async void TimerOnElapsed(object Sender, ElapsedEventArgs Args)
        {
            TimeSpan = TimeSpan.FromSeconds((int)_timing.Elapsed.TotalSeconds);
            
            // If Capture Duration is set and reached
            if (Duration > 0 && TimeSpan.TotalSeconds >= Duration)
            {
                if (_syncContext != null)
                    _syncContext.Post(async State => await StopRecording(), null);
                else await StopRecording();
            }
        }
        
        void CheckFunctionalityAvailability()
        {
            var audioAvailable = AudioSource.AudioAvailable;

            var videoAvailable = !(VideoViewModel.SelectedVideoSourceKind is NoVideoSourceProvider);
            
            RecordCommand.RaiseCanExecuteChanged(audioAvailable || videoAvailable);

            ScreenShotCommand.RaiseCanExecuteChanged(videoAvailable);
        }

        public void SaveScreenShot(Bitmap Bmp, string FileName = null)
        {
            // Save to Disk or Clipboard
            if (Bmp != null)
            {
                VideoViewModel.SelectedImageWriter.Save(Bmp, SelectedScreenShotImageFormat, FileName, Status, RecentViewModel);
            }
            else _systemTray.ShowTextNotification(Loc.ImgEmpty, 5000, null);
        }

        public Bitmap ScreenShotWindow(Window hWnd)
        {
            _systemTray.HideNotification();

            if (hWnd == Window.DesktopWindow)
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

            Bitmap bmp = null;

            var selectedVideoSource = VideoViewModel.SelectedVideoSource;
            var includeCursor = Settings.IncludeCursor;

            switch (VideoViewModel.SelectedVideoSourceKind)
            {
                case WindowSourceProvider _:
                    var hWnd = (selectedVideoSource as WindowItem)?.Window ?? Window.DesktopWindow;

                    bmp = ScreenShotWindow(hWnd);
                    break;

                case DeskDuplSourceProvider _:
                case ScreenSourceProvider _:
                    if (selectedVideoSource is FullScreenItem)
                    {
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
                    }
                    else if (selectedVideoSource is ScreenItem screen)
                    {
                        bmp = screen.Capture(includeCursor);
                    }
                    
                    bmp = bmp?.Transform(Settings.ScreenShots);
                    break;

                case RegionSourceProvider _:
                    bmp = ScreenShot.Capture(_regionProvider.SelectedRegion, includeCursor);
                    bmp = bmp.Transform(Settings.ScreenShots);
                    break;
            }

            SaveScreenShot(bmp, FileName);
        }
        
        public bool StartRecording(string FileName = null)
        {
            if (VideoViewModel.SelectedVideoWriterKind is FFMpegWriterProvider ||
                VideoViewModel.SelectedVideoWriterKind is StreamingWriterProvider ||
                (VideoViewModel.SelectedVideoSourceKind is NoVideoSourceProvider && VideoViewModel.SelectedVideoSource is FFMpegAudioItem))
            {
                if (!FFMpegService.FFMpegExists)
                {
                    ServiceProvider.MessageProvider.ShowFFMpegUnavailable();

                    return false;
                }

                FFMpegLog.Reset();
            }

            if (VideoViewModel.SelectedVideoWriterKind is GifWriterProvider
                && Settings.Gif.VariableFrameRate
                && VideoViewModel.SelectedVideoSourceKind is DeskDuplSourceProvider)
            {
                ServiceProvider.MessageProvider.ShowError("Using Variable Frame Rate GIF with Desktop Duplication is not supported.");

                return false;
            }

            if (VideoViewModel.SelectedVideoWriterKind is GifWriterProvider)
            {
                if (AudioSource.AudioAvailable)
                {
                    if (!ServiceProvider.MessageProvider.ShowYesNo("Audio won't be included in the recording.\nDo you want to record?", "Gif does not support Audio"))
                    {
                        return false;
                    }
                }
            }

            if (Duration != 0 && (StartDelay > Duration * 1000))
            {
                ServiceProvider.MessageProvider.ShowError(Loc.DelayGtDuration);

                return false;
            }

            IImageProvider imgProvider;

            try
            {
                imgProvider = GetImageProvider();
            }
            catch (NotSupportedException e) when (VideoViewModel.SelectedVideoSourceKind is DeskDuplSourceProvider)
            {
                var yes = ServiceProvider.MessageProvider.ShowYesNo($"{e.Message}\n\nDo you want to turn off Desktop Duplication.", Loc.ErrorOccured);

                if (yes)
                    VideoViewModel.SelectedVideoSourceKind = VideoViewModel.AvailableVideoSourceKinds[0];

                return false;
            }
            catch (Exception e)
            {
                ServiceProvider.MessageProvider.ShowError(e.ToString(), e.Message);

                return false;
            }
            
            Settings.EnsureOutPath();
            
            _isVideo = !(VideoViewModel.SelectedVideoSourceKind is NoVideoSourceProvider);
            
            var extension = VideoViewModel.SelectedVideoWriter.Extension;

            if (VideoViewModel.SelectedVideoSource is NoVideoItem x)
                extension = x.Extension;

            _currentFileName = FileName ?? Path.Combine(Settings.OutPath, $"{DateTime.Now:yyyy-MM-dd-HH-mm-ss}{extension}");

            IAudioProvider audioProvider;

            try
            {
                audioProvider = AudioSource.GetAudioProvider();
            }
            catch (Exception e)
            {
                ServiceProvider.MessageProvider.ShowError(e.ToString(), e.Message);

                imgProvider?.Dispose();

                return false;
            }

            IVideoFileWriter videoEncoder;

            try
            {
                videoEncoder = GetVideoFileWriter(imgProvider, audioProvider);
            }
            catch (Exception e)
            {
                ServiceProvider.MessageProvider.ShowError(e.ToString(), e.Message);

                imgProvider?.Dispose();
                audioProvider?.Dispose();

                return false;
            }

            if (videoEncoder is GifWriter gif && Settings.Gif.VariableFrameRate)
            {
                _recorder = new VFRGifRecorder(gif, imgProvider);
            }
            else if (_isVideo)
            {
                _recorder = new Recorder(videoEncoder, imgProvider, Settings.Video.FrameRate, audioProvider);
            }
            else if (VideoViewModel.SelectedVideoSource is NoVideoItem audioWriter)
            {
                _recorder = new Recorder(audioWriter.GetAudioFileWriter(_currentFileName, audioProvider.WaveFormat, Settings.Audio.Quality), audioProvider);
            }

            _regionProvider.Lock();

            _systemTray.HideNotification();

            if (Settings.UI.MinimizeOnStart)
                _mainWindow.IsMinimized = true;

            RecorderState = RecorderState.Recording;

            _timer?.Stop();
            TimeSpan = TimeSpan.Zero;

            Status.LocalizationKey = StartDelay > 0 ? nameof(LanguageManager.Waiting) : nameof(LanguageManager.Recording);

            _recorder.ErrorOccured += E =>
            {
                if (_syncContext != null)
                    _syncContext.Post(S => OnErrorOccured(E), null);
                else OnErrorOccured(E);
            };
            
            if (StartDelay > 0)
            {
                Task.Factory.StartNew(async () =>
                {
                    await Task.Delay(StartDelay);

                    _recorder.Start();
                });
            }
            else _recorder.Start();

            _timing?.Start();
            _timer?.Start();

            return true;
        }

        void OnErrorOccured(Exception E)
        {
            var cancelled = E is OperationCanceledException;

            if (!cancelled)
                Status.LocalizationKey = nameof(LanguageManager.ErrorOccured);
                        
            AfterRecording();

            if (!cancelled)
                ServiceProvider.MessageProvider.ShowError(E.ToString(), E.Message);
        }

        void AfterRecording()
        {
            RecorderState = RecorderState.NotRecording;

            _recorder = null;

            _timer?.Stop();
            _timing.Stop();
            
            if (Settings.UI.MinimizeOnStart)
                _mainWindow.IsMinimized = false;

            if (VideoViewModel.SelectedVideoSourceKind is RegionSourceProvider)
                _regionProvider.Release();
        }

        IVideoFileWriter GetVideoFileWriter(IImageProvider ImgProvider, IAudioProvider AudioProvider)
        {
            if (VideoViewModel.SelectedVideoSourceKind is NoVideoSourceProvider)
                return null;
            
            return VideoViewModel.SelectedVideoWriter.GetVideoFileWriter(_currentFileName,
                Settings.Video.FrameRate,
                Settings.Video.Quality,
                ImgProvider,
                Settings.Audio.Quality,
                AudioProvider);
        }
        
        IImageProvider GetImageProvider()
        {
            Func<Point, Point> transform = P => P;

            var imageProvider = VideoViewModel.SelectedVideoSource?.GetImageProvider(Settings.IncludeCursor, out transform);

            if (imageProvider == null)
                return null;

            var overlays = new List<IOverlay> { _webcamOverlay };

            if (MouseKeyHookAvailable)
                overlays.Add(new MouseKeyHook(Settings.Clicks, Settings.Keystrokes));
            
            // Custom Overlays
            overlays.AddRange(CustomOverlays.Collection.Where(M => M.Display).Select(M => new CustomOverlay(M, () => TimeSpan)));

            // Custom Image Overlays
            foreach (var overlay in CustomImageOverlays.Collection.Where(M => M.Display))
            {
                try
                {
                    overlays.Add(new CustomImageOverlay(overlay));
                }
                catch
                {
                    // Ignore Errors like Image not found, Invalid Image
                }
            }
            

            return new OverlayedImageProvider(imageProvider, transform, overlays.ToArray());
        }
        
        public async Task StopRecording()
        {
            Status.LocalizationKey = nameof(LanguageManager.Stopped);

            RecentItemViewModel savingRecentItem = null;

            // Assume saving to file only when extension is present
            if (!string.IsNullOrWhiteSpace(VideoViewModel.SelectedVideoWriter.Extension))
            {
                savingRecentItem = RecentViewModel.Add(_currentFileName, _isVideo ? RecentItemType.Video : RecentItemType.Audio, true);
            }
            
            // Reference Recorder as it will be set to null
            var rec = _recorder;
            
            var task = Task.Run(() => rec.Dispose());
            
            AfterRecording();

            try
            {
                // Ensure saved
                await task;
            }
            catch (Exception e)
            {
                ServiceProvider.MessageProvider.ShowError($"Error occured when stopping recording.\nThis might sometimes occur if you stop recording just as soon as you start it.\n\n{e}");

                return;
            }

            if (savingRecentItem != null)
            {
                // After Save
                savingRecentItem.Saved();

                if (Settings.CopyOutPathToClipboard)
                    savingRecentItem.FilePath.WriteToClipboard();

                _systemTray.ShowTextNotification((savingRecentItem.ItemType == RecentItemType.Video ? Loc.VideoSaved : Loc.AudioSaved) + ": " +
                    Path.GetFileName(savingRecentItem.FilePath), 5000, () =>
                    {
                        ServiceProvider.LaunchFile(new ProcessStartInfo(savingRecentItem.FilePath));
                    });
            }
        }
    }
}