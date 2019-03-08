using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Captura.Audio;
using Captura.FFmpeg;
using Captura.Models;
using Captura.Webcam;
using Microsoft.Win32;
using Screna;

namespace Captura.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class RecordingModel : ViewModelBase, IDisposable
    {
        #region Fields
        IRecorder _recorder;
        string _currentFileName;
        bool _isVideo;

        readonly SynchronizationContext _syncContext = SynchronizationContext.Current;

        readonly ISystemTray _systemTray;
        readonly WebcamOverlay _webcamOverlay;
        readonly IMainWindow _mainWindow;
        readonly IPreviewWindow _previewWindow;
        readonly WebcamModel _webcamModel;
        readonly IAudioPlayer _audioPlayer;
        readonly TimerModel _timerModel;
        readonly IMessageProvider _messageProvider;
        readonly IFFmpegViewsProvider _ffmpegViewsProvider;

        readonly KeymapViewModel _keymap;

        readonly VideoSourcesViewModel _videoSourcesViewModel;
        readonly VideoWritersViewModel _videoWritersViewModel;
        readonly AudioSource _audioSource;
        readonly IRecentList _recentList;
        #endregion

        public RecordingModel(Settings Settings,
            ILocalizationProvider Loc,
            ISystemTray SystemTray,
            WebcamOverlay WebcamOverlay,
            IMainWindow MainWindow,
            IPreviewWindow PreviewWindow,
            VideoSourcesViewModel VideoSourcesViewModel,
            VideoWritersViewModel VideoWritersViewModel,
            AudioSource AudioSource,
            WebcamModel WebcamModel,
            KeymapViewModel Keymap,
            IAudioPlayer AudioPlayer,
            IRecentList RecentList,
            TimerModel TimerModel,
            IMessageProvider MessageProvider,
            IFFmpegViewsProvider FFmpegViewsProvider) : base(Settings, Loc)
        {
            _systemTray = SystemTray;
            _webcamOverlay = WebcamOverlay;
            _mainWindow = MainWindow;
            _previewWindow = PreviewWindow;
            _videoSourcesViewModel = VideoSourcesViewModel;
            _videoWritersViewModel = VideoWritersViewModel;
            _audioSource = AudioSource;
            _webcamModel = WebcamModel;
            _keymap = Keymap;
            _audioPlayer = AudioPlayer;
            _recentList = RecentList;
            _timerModel = TimerModel;
            _messageProvider = MessageProvider;
            _ffmpegViewsProvider = FFmpegViewsProvider;

            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;

            TimerModel.CountdownElapsed += InternalStartRecording;
            TimerModel.DurationElapsed += async () =>
            {
                if (_syncContext != null)
                    _syncContext.Post(async State => await StopRecording(), null);
                else await StopRecording();
            };
        }

        RecorderState _recorderState = RecorderState.NotRecording;

        public RecorderState RecorderState
        {
            get => _recorderState;
            private set => Set(ref _recorderState, value);
        }

        public async void OnRecordExecute()
        {
            if (RecorderState == RecorderState.NotRecording)
            {
                _audioPlayer.Play(SoundKind.Start);

                StartRecording();
            }
            else
            {
                _audioPlayer.Play(SoundKind.Stop);

                await StopRecording();
            }
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

        INotification _pauseNotification;

        public void OnPauseExecute()
        {
            _audioPlayer.Play(SoundKind.Pause);

            // Resume
            if (RecorderState == RecorderState.Paused)
            {
                _systemTray.HideNotification();

                _recorder.Start();
                _timerModel.Resume();

                RecorderState = RecorderState.Recording;

                _pauseNotification?.Remove();
            }
            else // Pause
            {
                _recorder.Stop();
                _timerModel.Pause();

                RecorderState = RecorderState.Paused;

                _pauseNotification = new TextNotification(Loc.Paused, OnPauseExecute);
                _systemTray.ShowNotification(_pauseNotification);
            }
        }

        bool CheckFFmpeg()
        {
            var isFFmpegVideoItem = _videoWritersViewModel.SelectedVideoWriterKind is FFmpegWriterProvider ||
                                    _videoWritersViewModel.SelectedVideoWriterKind is StreamingWriterProvider;

            var isFFmpegAudioItem =
                _videoSourcesViewModel.SelectedVideoSourceKind is NoVideoSourceProvider noVideoSourceProvider
                && noVideoSourceProvider.Source is NoVideoItem noVideoItem
                // HACK: Assuming non Wave writer is FFmpeg
                && !(noVideoItem.AudioWriterItem is WaveItem);

            if (isFFmpegVideoItem || isFFmpegAudioItem)
            {
                if (!FFmpegService.FFmpegExists)
                {
                    _ffmpegViewsProvider.ShowUnavailable();

                    return false;
                }
            }

            return true;
        }

        bool SetupVideoRecorder(IAudioProvider AudioProvider)
        {
            IImageProvider imgProvider;

            try
            {
                imgProvider = GetImageProvider();
            }
            catch (NotSupportedException e) when (_videoSourcesViewModel.SelectedVideoSourceKind is DeskDuplSourceProvider)
            {
                var yes = _messageProvider.ShowYesNo($"{e.Message}\n\nDo you want to turn off Desktop Duplication?", Loc.ErrorOccurred);

                if (yes)
                    _videoSourcesViewModel.SetDefaultSource();

                return false;
            }
            catch (WindowClosedException)
            {
                _messageProvider.ShowError("Selected Window has been Closed.", "Window Closed");

                return false;
            }
            catch (Exception e)
            {
                _messageProvider.ShowException(e, e.Message);

                return false;
            }

            IVideoFileWriter videoEncoder;

            try
            {
                videoEncoder = GetVideoFileWriterWithPreview(imgProvider, AudioProvider);
            }
            catch (Exception e)
            {
                _messageProvider.ShowException(e, e.Message);

                imgProvider?.Dispose();

                return false;
            }

            _recorder = new Recorder(videoEncoder, imgProvider, Settings.Video.FrameRate, AudioProvider);

            var webcamMode = _videoSourcesViewModel.SelectedVideoSourceKind is WebcamSourceProvider;

            // Separate file for webcam
            if (!webcamMode
                && !(_webcamModel.SelectedCam is NoWebcamItem)
                && Settings.WebcamOverlay.SeparateFile)
            {
                SeparateFileForWebcam();
            }

            // Separate file for every audio source
            if (Settings.Audio.Enabled
                && Settings.Audio.SeparateFilePerSource)
            {
                SeparateFileForEveryAudioSource();
            }

            return true;
        }

        bool SetupAudioProvider(out IAudioProvider AudioProvider)
        {
            AudioProvider = null;

            try
            {
                if (Settings.Audio.Enabled && !Settings.Audio.SeparateFilePerSource)
                {
                    AudioProvider = _audioSource.GetMixedAudioProvider();
                }
            }
            catch (Exception e)
            {
                _messageProvider.ShowException(e, e.Message);

                return false;
            }

            return true;
        }

        public bool StartRecording(string FileName = null)
        {
            Settings.EnsureOutPath();

            _isVideo = !(_videoSourcesViewModel.SelectedVideoSourceKind is NoVideoSourceProvider);

            var extension = _videoWritersViewModel.SelectedVideoWriter.Extension;

            if (_videoSourcesViewModel.SelectedVideoSourceKind?.Source is NoVideoItem x)
                extension = x.Extension;

            _currentFileName = Settings.GetFileName(extension, FileName);

            if (!CheckFFmpeg())
                return false;

            if (!SetupAudioProvider(out var audioProvider))
                return false;

            if (_isVideo)
            {
                if (!SetupVideoRecorder(audioProvider))
                {
                    audioProvider?.Dispose();

                    return false;
                }
            }
            else if (_videoSourcesViewModel.SelectedVideoSourceKind?.Source is NoVideoItem audioWriter)
            {
                if (!InitAudioRecorder(audioWriter, audioProvider))
                {
                    audioProvider?.Dispose();

                    return false;
                }
            }

            _systemTray.HideNotification();

            if (Settings.Tray.MinToTrayOnCaptureStart)
                _mainWindow.IsVisible = false;

            RecorderState = RecorderState.Recording;

            _recorder.ErrorOccurred += OnErrorOccured;

            if (Settings.PreStartCountdown == 0)
            {
                InternalStartRecording();
            }

            _timerModel.Start();

            return true;
        }

        IRecorder GetAudioRecorder(NoVideoItem AudioWriter, IAudioProvider AudioProvider, string AudioFileName = null)
        {
            var audioFileWriter = AudioWriter.AudioWriterItem.GetAudioFileWriter(
                AudioFileName ?? _currentFileName,
                AudioProvider?.WaveFormat,
                Settings.Audio.Quality);

            return new Recorder(audioFileWriter, AudioProvider);
        }

        string GetAudioFileName(int Index)
        {
            return Path.ChangeExtension(_currentFileName,
                $".{Index}{Path.GetExtension(_currentFileName)}");
        }

        bool InitAudioRecorder(NoVideoItem AudioWriter, IAudioProvider AudioProvider)
        {
            if (!Settings.Audio.SeparateFilePerSource)
            {
                _recorder = GetAudioRecorder(AudioWriter, AudioProvider);
            }
            else
            {
                var audioProviders = _audioSource.GetMultipleAudioProviders();

                if (audioProviders.Length > 0)
                {
                    var recorders = audioProviders
                        .Select((M, Index) => GetAudioRecorder(AudioWriter, M, GetAudioFileName(Index)))
                        .ToArray();

                    _recorder = new MultiRecorder(recorders);

                    // Set to first file
                    _currentFileName = GetAudioFileName(0);
                }
                else
                {
                    _messageProvider.ShowError("No Audio Sources selected");

                    return false;
                }
            }

            return true;
        }

        void SeparateFileForWebcam()
        {
            var webcamImgProvider = new WebcamImageProvider(_webcamModel);

            var webcamFileName = Path.ChangeExtension(_currentFileName, $".webcam{Path.GetExtension(_currentFileName)}");

            var webcamVideoWriter = GetVideoFileWriter(webcamImgProvider, null, webcamFileName);

            var webcamRecorder = new Recorder(webcamVideoWriter, webcamImgProvider, Settings.Video.FrameRate);

            _recorder = new MultiRecorder(_recorder, webcamRecorder);
        }

        void SeparateFileForEveryAudioSource()
        {
            var audioWriter = new WaveItem();

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

        void InternalStartRecording()
        {
            _recorder.Start();
        }

        void OnErrorOccured(Exception E)
        {
            void Do()
            {
                var cancelled = E is WindowClosedException;

                AfterRecording();

                if (!cancelled)
                    _messageProvider.ShowException(E, E.Message);

                if (cancelled)
                {
                    _videoSourcesViewModel.SetDefaultSource();
                }
            }

            if (_syncContext != null)
                _syncContext.Post(S => Do(), null);
            else Do();
        }

        void AfterRecording()
        {
            _pauseNotification?.Remove();

            RecorderState = RecorderState.NotRecording;

            _recorder.ErrorOccurred -= OnErrorOccured;
            _recorder = null;

            _timerModel.Stop();
        }

        IVideoFileWriter GetVideoFileWriterWithPreview(IImageProvider ImgProvider, IAudioProvider AudioProvider)
        {
            return _videoSourcesViewModel.SelectedVideoSourceKind is NoVideoSourceProvider
                ? null
                : new WithPreviewWriter(GetVideoFileWriter(ImgProvider, AudioProvider), _previewWindow);
        }

        IVideoFileWriter GetVideoFileWriter(IImageProvider ImgProvider, IAudioProvider AudioProvider, string FileName = null)
        {
            if (_videoSourcesViewModel.SelectedVideoSourceKind is NoVideoSourceProvider)
                return null;

            return _videoWritersViewModel.SelectedVideoWriter.GetVideoFileWriter(new VideoWriterArgs
            {
                FileName = FileName ?? _currentFileName,
                FrameRate = Settings.Video.FrameRate,
                VideoQuality = Settings.Video.Quality,
                ImageProvider = ImgProvider,
                AudioQuality = Settings.Audio.Quality,
                AudioProvider = AudioProvider
            });
        }

        IEnumerable<IOverlay> GetOverlays()
        {
            // No mouse and webcam overlays in webcam mode
            var webcamMode = _videoSourcesViewModel.SelectedVideoSourceKind is WebcamSourceProvider;

            yield return new CensorOverlay(Settings.Censored);

            if (!webcamMode && !Settings.WebcamOverlay.SeparateFile)
            {
                yield return _webcamOverlay;
            }

            if (!webcamMode)
            {
                yield return new MousePointerOverlay(Settings.MousePointerOverlay);
            }

            var clickSettings = webcamMode
                ? new MouseClickSettings { Display = false }
                : Settings.Clicks;

            yield return new MouseKeyHook(clickSettings,
                Settings.Keystrokes,
                _keymap,
                _currentFileName,
                () => _timerModel.TimeSpan);

            yield return new ElapsedOverlay(Settings.Elapsed, () => _timerModel.TimeSpan);

            // Text Overlays
            foreach (var overlay in Settings.TextOverlays)
            {
                yield return new CustomOverlay(overlay);
            }

            // Image Overlays
            foreach (var overlay in Settings.ImageOverlays.Where(M => M.Display))
            {
                IOverlay imgOverlay = null;

                try
                {
                    imgOverlay = new CustomImageOverlay(overlay);
                }
                catch
                {
                    // Ignore Errors like Image not found, Invalid Image
                }

                if (imgOverlay != null)
                    yield return imgOverlay;
            }
        }

        IImageProvider GetImageProvider()
        {
            Func<Point, Point> transform = P => P;

            var imageProvider = _videoSourcesViewModel
                .SelectedVideoSourceKind
                ?.Source
                ?.GetImageProvider(Settings.IncludeCursor, out transform);

            return imageProvider == null
                ? null
                : new OverlayedImageProvider(imageProvider, transform, GetOverlays().ToArray());
        }

        readonly object _stopRecTaskLock = new object();
        readonly List<Task> _stopRecTasks = new List<Task>();

        int RunningStopRecordingCount
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
            FileRecentItem savingRecentItem = null;
            FileSaveNotification notification = null;

            // Reference current file name
            var fileName = _currentFileName;

            // Assume saving to file only when extension is present
            if (!_timerModel.Waiting && !string.IsNullOrWhiteSpace(_videoWritersViewModel.SelectedVideoWriter.Extension))
            {
                savingRecentItem = new FileRecentItem(_currentFileName, _isVideo ? RecentFileType.Video : RecentFileType.Audio, true);
                _recentList.Add(savingRecentItem);

                notification = new FileSaveNotification(savingRecentItem);

                notification.OnDelete += () => savingRecentItem.RemoveCommand.ExecuteIfCan();

                _systemTray.ShowNotification(notification);
            }

            // Reference Recorder as it will be set to null
            var rec = _recorder;

            var task = Task.Run(() => rec.Dispose());

            lock (_stopRecTaskLock)
            {
                _stopRecTasks.Add(task);
            }

            AfterRecording();

            var wasWaiting = _timerModel.Waiting;
            _timerModel.Waiting = false;

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
                _messageProvider.ShowException(e, "Error occurred when stopping recording.\nThis might sometimes occur if you stop recording just as soon as you start it.");

                return;
            }

            if (wasWaiting)
            {
                try
                {
                    File.Delete(fileName);
                }
                catch
                {
                    // Ignore Errors
                }
            }

            if (savingRecentItem != null)
            {
                AfterSave(savingRecentItem, notification);
            }
        }

        void AfterSave(FileRecentItem SavingRecentItem, FileSaveNotification Notification)
        {
            SavingRecentItem.Saved();
        
            if (Settings.CopyOutPathToClipboard)
                SavingRecentItem.FileName.WriteToClipboard();

            Notification.Saved();
        }

        public bool CanExit()
        {
            if (RecorderState == RecorderState.Recording)
            {
                if (!_messageProvider.ShowYesNo(
                    "A Recording is in progress. Are you sure you want to exit?", "Confirm Exit"))
                    return false;
            }
            else if (RunningStopRecordingCount > 0)
            {
                if (!_messageProvider.ShowYesNo(
                    "Some Recordings have not finished writing to disk. Are you sure you want to exit?", "Confirm Exit"))
                    return false;
            }

            return true;
        }
    }
}