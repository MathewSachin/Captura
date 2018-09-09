using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Captura.Audio;
using Captura.Models;
using Captura.Webcam;
using Microsoft.Win32;
using Screna;
using Timer = System.Timers.Timer;

namespace Captura.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class RecordingViewModel : ViewModelBase, IDisposable
    {
        #region Fields
        Timer _timer;
        readonly Timing _timing = new Timing();
        IRecorder _recorder;
        string _currentFileName;
        bool _isVideo;

        readonly SynchronizationContext _syncContext = SynchronizationContext.Current;

        readonly ISystemTray _systemTray;
        readonly IRegionProvider _regionProvider;
        readonly WebcamOverlay _webcamOverlay;
        readonly IMainWindow _mainWindow;
        readonly IPreviewWindow _previewWindow;
        readonly IWebCamProvider _webCamProvider;

        readonly KeymapViewModel _keymap;

        readonly VideoViewModel _videoViewModel;
        readonly AudioSource _audioSource;
        readonly RecentViewModel _recentViewModel;

        public DelegateCommand RecordCommand { get; }
        public DelegateCommand PauseCommand { get; }

        public CustomOverlaysViewModel CustomOverlays { get; }
        public CustomImageOverlaysViewModel CustomImageOverlays { get; }
        public CensorOverlaysViewModel CensorOverlays { get; }

        public TextLocalizer Status { get; } = new TextLocalizer(nameof(LanguageManager.Ready));
        #endregion

        public RecordingViewModel(Settings Settings,
            LanguageManager LanguageManager,
            CustomOverlaysViewModel CustomOverlays,
            CustomImageOverlaysViewModel CustomImageOverlays,
            CensorOverlaysViewModel CensorOverlays,
            ISystemTray SystemTray,
            IRegionProvider RegionProvider,
            WebcamOverlay WebcamOverlay,
            IMainWindow MainWindow,
            IPreviewWindow PreviewWindow,
            VideoViewModel VideoViewModel,
            AudioSource AudioSource,
            RecentViewModel RecentViewModel,
            IWebCamProvider WebCamProvider,
            KeymapViewModel Keymap) : base(Settings, LanguageManager)
        {
            this.CustomOverlays = CustomOverlays;
            this.CustomImageOverlays = CustomImageOverlays;
            this.CensorOverlays = CensorOverlays;
            _systemTray = SystemTray;
            _regionProvider = RegionProvider;
            _webcamOverlay = WebcamOverlay;
            _mainWindow = MainWindow;
            _previewWindow = PreviewWindow;
            _videoViewModel = VideoViewModel;
            _audioSource = AudioSource;
            _recentViewModel = RecentViewModel;
            _webCamProvider = WebCamProvider;
            _keymap = Keymap;

            RecordCommand = new DelegateCommand(OnRecordExecute);

            PauseCommand = new DelegateCommand(OnPauseExecute, false);

            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
        }

        RecorderState _recorderState = RecorderState.NotRecording;

        public RecorderState RecorderState
        {
            get => _recorderState;
            private set
            {
                if (_recorderState == value)
                    return;

                _recorderState = value;

                PauseCommand.RaiseCanExecuteChanged(value != RecorderState.NotRecording);

                OnPropertyChanged();
            }
        }

        bool _canChangeWebcam = true, _canChangeAudioSources = true;

        public bool CanChangeWebcam
        {
            get => _canChangeWebcam;
            set
            {
                _canChangeWebcam = value;

                OnPropertyChanged();
            }
        }

        public bool CanChangeAudioSources
        {
            get => _canChangeAudioSources;
            set
            {
                _canChangeAudioSources = value;
                
                OnPropertyChanged();
            }
        }

        #region Time
        TimeSpan _ts;

        public TimeSpan TimeSpan
        {
            get => _ts;
            set
            {
                if (_ts == value)
                    return;

                _ts = value;

                OnPropertyChanged();
            }
        }
        #endregion

        async void OnRecordExecute()
        {
            if (RecorderState == RecorderState.NotRecording)
                StartRecording();
            else await StopRecording();
        }

        void SystemEvents_PowerModeChanged(object Sender, PowerModeChangedEventArgs E)
        {
            if (E.Mode == PowerModes.Suspend && RecorderState == RecorderState.Recording)
            {
                OnPauseExecute();
            }
        }

        public void Dispose()
        {
            SystemEvents.PowerModeChanged -= SystemEvents_PowerModeChanged;
        }

        void OnPauseExecute()
        {
            // Resume
            if (RecorderState == RecorderState.Paused)
            {
                _systemTray.HideNotification();

                _recorder.Start();
                _timing?.Start();
                _timer?.Start();

                RecorderState = RecorderState.Recording;
                Status.LocalizationKey = nameof(LanguageManager.Recording);
            }
            else // Pause
            {
                _recorder.Stop();
                _timer?.Stop();
                _timing?.Pause();

                RecorderState = RecorderState.Paused;
                Status.LocalizationKey = nameof(LanguageManager.Paused);

                _systemTray.ShowTextNotification(Loc.Paused, null);
            }
        }

        public void InitTimer()
        {
            _timer = new Timer(500);
            _timer.Elapsed += TimerOnElapsed;
        }

        async void TimerOnElapsed(object Sender, ElapsedEventArgs Args)
        {
            TimeSpan = TimeSpan.FromSeconds((int)_timing.Elapsed.TotalSeconds);

            var duration = Settings.Duration;

            // If Capture Duration is set and reached
            if (duration > 0 && TimeSpan.TotalSeconds >= Settings.StartDelay / 1000 + duration)
            {
                if (_syncContext != null)
                    _syncContext.Post(async State => await StopRecording(), null);
                else await StopRecording();
            }
        }

        public bool StartRecording(string FileName = null)
        {
            if (_videoViewModel.SelectedVideoWriterKind is FFmpegWriterProvider ||
                _videoViewModel.SelectedVideoWriterKind is StreamingWriterProvider ||
                (_videoViewModel.SelectedVideoSourceKind is NoVideoSourceProvider noVideoSourceProvider
                && noVideoSourceProvider.Source is FFmpegAudioItem))
            {
                if (!FFmpegService.FFmpegExists)
                {
                    ServiceProvider.MessageProvider.ShowFFmpegUnavailable();

                    return false;
                }
            }

            if (_videoViewModel.SelectedVideoWriterKind is GifWriterProvider)
            {
                if (Settings.Audio.Enabled)
                {
                    if (!ServiceProvider.MessageProvider.ShowYesNo("Audio won't be included in the recording.\nDo you want to record?", "Gif does not support Audio"))
                    {
                        return false;
                    }
                }
            }

            IImageProvider imgProvider;

            try
            {
                imgProvider = GetImageProvider();
            }
            catch (NotSupportedException e) when (_videoViewModel.SelectedVideoSourceKind is DeskDuplSourceProvider)
            {
                var yes = ServiceProvider.MessageProvider.ShowYesNo($"{e.Message}\n\nDo you want to turn off Desktop Duplication.", Loc.ErrorOccurred);

                if (yes)
                    _videoViewModel.SetDefaultSource();

                return false;
            }
            catch (Exception e)
            {
                ServiceProvider.MessageProvider.ShowException(e, e.Message);

                return false;
            }

            if (_videoViewModel.SelectedVideoWriterKind is GifWriterProvider
                && Settings.Gif.VariableFrameRate
                && imgProvider is DeskDuplImageProvider deskDuplImageProvider)
            {
                deskDuplImageProvider.Timeout = 5000;
            }

            Settings.EnsureOutPath();

            _isVideo = !(_videoViewModel.SelectedVideoSourceKind is NoVideoSourceProvider);

            var extension = _videoViewModel.SelectedVideoWriter.Extension;

            if (_videoViewModel.SelectedVideoSourceKind?.Source is NoVideoItem x)
                extension = x.Extension;

            _currentFileName = FileName ?? Path.Combine(Settings.OutPath, $"{DateTime.Now:yyyy-MM-dd-HH-mm-ss}{extension}");

            IAudioProvider audioProvider = null;

            try
            {
                if (Settings.Audio.Enabled && !Settings.Audio.SeparateFilePerSource)
                {
                    audioProvider = _audioSource.GetMixedAudioProvider();
                }
            }
            catch (Exception e)
            {
                ServiceProvider.MessageProvider.ShowException(e, e.Message);

                imgProvider?.Dispose();

                return false;
            }

            IVideoFileWriter videoEncoder;

            try
            {
                videoEncoder = GetVideoFileWriterWithPreview(imgProvider, audioProvider);
            }
            catch (Exception e)
            {
                ServiceProvider.MessageProvider.ShowException(e, e.Message);

                imgProvider?.Dispose();
                audioProvider?.Dispose();

                return false;
            }

            switch (videoEncoder)
            {
                case GifWriter gif when Settings.Gif.VariableFrameRate:
                    _recorder = new VFRGifRecorder(gif, imgProvider);
                    break;

                case WithPreviewWriter previewWriter when previewWriter.OriginalWriter is GifWriter gif && Settings.Gif.VariableFrameRate:
                    _recorder = new VFRGifRecorder(gif, imgProvider);
                    break;

                default:
                    if (_isVideo)
                    {
                        _recorder = new Recorder(videoEncoder, imgProvider, Settings.Video.FrameRate, audioProvider);
                    }
                    else if (_videoViewModel.SelectedVideoSourceKind?.Source is NoVideoItem audioWriter)
                    {
                        IRecorder GetAudioRecorder(IAudioProvider AudioProvider, string AudioFileName = null)
                        {
                            return new Recorder(
                                audioWriter.GetAudioFileWriter(AudioFileName ?? _currentFileName, AudioProvider?.WaveFormat,
                                    Settings.Audio.Quality), AudioProvider);
                        }

                        if (!Settings.Audio.SeparateFilePerSource)
                        {
                            _recorder = GetAudioRecorder(audioProvider);
                        }
                        else
                        {
                            string GetAudioFileName(int Index)
                            {
                                return Path.ChangeExtension(_currentFileName,
                                    $".{Index}{Path.GetExtension(_currentFileName)}");
                            }

                            var audioProviders = _audioSource.GetMultipleAudioProviders();

                            if (audioProviders.Length > 0)
                            {
                                var recorders = audioProviders
                                    .Select((M, Index) => GetAudioRecorder(M, GetAudioFileName(Index)))
                                    .ToArray();

                                _recorder = new MultiRecorder(recorders);

                                // Set to first file
                                _currentFileName = GetAudioFileName(0);
                            }
                            else
                            {
                                ServiceProvider.MessageProvider.ShowError("No Audio Sources selected");

                                return false;
                            }
                        }
                    }
                    break;
            }

            // Separate file for webcam
            if (_isVideo && _webCamProvider.SelectedCam != WebcamItem.NoWebcam && Settings.WebcamOverlay.SeparateFile)
            {
                var webcamImgProvider = new WebcamImageProvider(_webCamProvider);

                var webcamFileName = Path.ChangeExtension(_currentFileName, $".webcam{Path.GetExtension(_currentFileName)}");

                var webcamVideoWriter = GetVideoFileWriter(webcamImgProvider, null, webcamFileName);

                var webcamRecorder = new Recorder(webcamVideoWriter, webcamImgProvider, Settings.Video.FrameRate);

                _recorder = new MultiRecorder(_recorder, webcamRecorder);
            }

            // Separate file for every audio source
            if (_isVideo && Settings.Audio.Enabled && Settings.Audio.SeparateFilePerSource)
            {
                var audioWriter = WaveItem.Instance;
                
                IRecorder GetAudioRecorder(IAudioProvider AudioProvider, string AudioFileName = null)
                {
                    return new Recorder(
                        audioWriter.GetAudioFileWriter(AudioFileName ?? _currentFileName, AudioProvider?.WaveFormat,
                            Settings.Audio.Quality), AudioProvider);
                }

                string GetAudioFileName(int Index)
                {
                    return Path.ChangeExtension(_currentFileName, $".{Index}.wav");
                }

                var audioProviders = _audioSource.GetMultipleAudioProviders();

                if (audioProviders.Length > 0)
                {
                    var recorders = audioProviders
                        .Select((M, Index) => GetAudioRecorder(M, GetAudioFileName(Index)))
                        .Concat(new[] {_recorder})
                        .ToArray();

                    _recorder = new MultiRecorder(recorders);
                }
            }

            if (_videoViewModel.SelectedVideoSourceKind is RegionSourceProvider)
                _regionProvider.Lock();

            _systemTray.HideNotification();

            if (Settings.UI.MinimizeOnStart)
                _mainWindow.IsMinimized = true;

            RecorderState = RecorderState.Recording;

            CanChangeWebcam = !Settings.WebcamOverlay.SeparateFile;
            CanChangeAudioSources = !Settings.Audio.SeparateFilePerSource;

            _timer?.Stop();
            TimeSpan = TimeSpan.Zero;

            Status.LocalizationKey = Settings.StartDelay > 0 ? nameof(LanguageManager.Waiting) : nameof(LanguageManager.Recording);

            _recorder.ErrorOccurred += E =>
            {
                if (_syncContext != null)
                    _syncContext.Post(S => OnErrorOccurred(E), null);
                else OnErrorOccurred(E);
            };

            if (Settings.StartDelay > 0)
            {
                Task.Factory.StartNew(async () =>
                {
                    await Task.Delay(Settings.StartDelay);

                    Status.LocalizationKey = nameof(LanguageManager.Recording);

                    _recorder.Start();
                });
            }
            else _recorder.Start();

            _timing?.Start();
            _timer?.Start();

            return true;
        }

        void OnErrorOccurred(Exception E)
        {
            var cancelled = E is OperationCanceledException;

            if (!cancelled)
                Status.LocalizationKey = nameof(LanguageManager.ErrorOccurred);

            AfterRecording();

            if (!cancelled)
                ServiceProvider.MessageProvider.ShowException(E, E.Message);
        }

        void AfterRecording()
        {
            RecorderState = RecorderState.NotRecording;

            CanChangeWebcam = CanChangeAudioSources = true;

            _recorder = null;

            _timer?.Stop();
            _timing.Stop();

            if (Settings.UI.MinimizeOnStart)
                _mainWindow.IsMinimized = false;

            if (_videoViewModel.SelectedVideoSourceKind is RegionSourceProvider)
                _regionProvider.Release();
        }

        IVideoFileWriter GetVideoFileWriterWithPreview(IImageProvider ImgProvider, IAudioProvider AudioProvider)
        {
            if (_videoViewModel.SelectedVideoSourceKind is NoVideoSourceProvider)
                return null;

            _previewWindow.Init(ImgProvider.Width, ImgProvider.Height);

            return new WithPreviewWriter(GetVideoFileWriter(ImgProvider, AudioProvider), _previewWindow);
        }

        IVideoFileWriter GetVideoFileWriter(IImageProvider ImgProvider, IAudioProvider AudioProvider, string FileName = null)
        {
            if (_videoViewModel.SelectedVideoSourceKind is NoVideoSourceProvider)
                return null;

            return _videoViewModel.SelectedVideoWriter.GetVideoFileWriter(new VideoWriterArgs
            {
                FileName = FileName ?? _currentFileName,
                FrameRate = Settings.Video.FrameRate,
                VideoQuality = Settings.Video.Quality,
                ImageProvider = ImgProvider,
                AudioQuality = Settings.Audio.Quality,
                AudioProvider = AudioProvider
            });
        }

        IImageProvider GetImageProvider()
        {
            Func<Point, Point> transform = P => P;

            var imageProvider = _videoViewModel.SelectedVideoSourceKind?.Source?.GetImageProvider(Settings.IncludeCursor, out transform);

            if (imageProvider == null)
                return null;

            var overlays = new List<IOverlay>
            {
                new CensorOverlay(Settings.Censored)
            };

            if (!Settings.WebcamOverlay.SeparateFile)
            {
                overlays.Add(_webcamOverlay);
            }

            overlays.Add(new MousePointerOverlay(Settings.MousePointerOverlay));
            overlays.Add(new MouseKeyHook(Settings.Clicks, Settings.Keystrokes, _keymap));
            overlays.Add(new ElapsedOverlay(Settings.Elapsed, () => TimeSpan));

            // Custom Overlays
            overlays.AddRange(CustomOverlays.Collection.Select(M => new CustomOverlay(M)));

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

        readonly object _stopRecTaskLock = new object();
        readonly List<Task> _stopRecTasks = new List<Task>();

        public int RunningStopRecordingCount
        {
            get
            {
                lock (_stopRecTaskLock)
                {
                    return _stopRecTasks.Count(M => !M.IsCompleted);
                }
            }
        }

        public async Task StopRecording()
        {
            Status.LocalizationKey = nameof(LanguageManager.Stopped);

            RecentItemViewModel savingRecentItem = null;

            // Assume saving to file only when extension is present
            if (!string.IsNullOrWhiteSpace(_videoViewModel.SelectedVideoWriter.Extension))
            {
                savingRecentItem = _recentViewModel.Add(_currentFileName, _isVideo ? RecentItemType.Video : RecentItemType.Audio, true);
            }

            // Reference Recorder as it will be set to null
            var rec = _recorder;

            var task = Task.Run(() => rec.Dispose());

            lock (_stopRecTaskLock)
            {
                _stopRecTasks.Add(task);
            }

            AfterRecording();

            try
            {
                // Ensure saved
                await task;

                lock (_stopRecTaskLock)
                {
                    _stopRecTasks.Remove(task);
                }
            }
            catch (Exception e)
            {
                ServiceProvider.MessageProvider.ShowException(e, "Error occurred when stopping recording.\nThis might sometimes occur if you stop recording just as soon as you start it.");

                return;
            }

            if (savingRecentItem != null)
            {
                // After Save
                savingRecentItem.Saved();

                if (Settings.CopyOutPathToClipboard)
                    savingRecentItem.FilePath.WriteToClipboard();

                _systemTray.ShowTextNotification((savingRecentItem.ItemType == RecentItemType.Video ? Loc.VideoSaved : Loc.AudioSaved) + ": " +
                    Path.GetFileName(savingRecentItem.FilePath), () =>
                    {
                        ServiceProvider.LaunchFile(new ProcessStartInfo(savingRecentItem.FilePath));
                    });
            }
        }
    }
}