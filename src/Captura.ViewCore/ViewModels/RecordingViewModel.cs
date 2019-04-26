using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Captura.Models;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Screna;

namespace Captura.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class RecordingViewModel : ViewModelBase
    {
        readonly RecordingModel _recordingModel;
        readonly TimerModel _timerModel;
        readonly VideoSourcesViewModel _videoSourcesViewModel;
        readonly VideoWritersViewModel _videoWritersViewModel;
        readonly ISystemTray _systemTray;
        readonly IMainWindow _mainWindow;
        readonly IAudioPlayer _audioPlayer;
        readonly IRecentList _recentList;
        readonly IMessageProvider _messageProvider;
        readonly AudioSourceViewModel _audioSourceViewModel;

        readonly SynchronizationContext _syncContext = SynchronizationContext.Current;

        public ICommand RecordCommand { get; }
        public ICommand PauseCommand { get; }

        public RecordingViewModel(RecordingModel RecordingModel,
            Settings Settings,
            TimerModel TimerModel,
            WebcamModel WebcamModel,
            VideoSourcesViewModel VideoSourcesViewModel,
            VideoWritersViewModel VideoWritersViewModel,
            ISystemTray SystemTray,
            IMainWindow MainWindow,
            ILocalizationProvider Loc,
            IAudioPlayer AudioPlayer,
            IRecentList RecentList,
            IMessageProvider MessageProvider,
            AudioSourceViewModel AudioSourceViewModel) : base(Settings, Loc)
        {
            _recordingModel = RecordingModel;
            _timerModel = TimerModel;
            _videoSourcesViewModel = VideoSourcesViewModel;
            _videoWritersViewModel = VideoWritersViewModel;
            _systemTray = SystemTray;
            _mainWindow = MainWindow;
            _audioPlayer = AudioPlayer;
            _recentList = RecentList;
            _messageProvider = MessageProvider;
            _audioSourceViewModel = AudioSourceViewModel;

            RecordCommand = new[]
                {
                    Settings.Audio
                        .ObserveProperty(M => M.Enabled),
                    VideoSourcesViewModel
                        .ObserveProperty(M => M.SelectedVideoSourceKind)
                        .Select(M => M is NoVideoSourceProvider),
                    VideoSourcesViewModel
                        .ObserveProperty(M => M.SelectedVideoSourceKind)
                        .Select(M => M is WebcamSourceProvider),
                    WebcamModel
                        .ObserveProperty(M => M.SelectedCam)
                        .Select(M => M is NoWebcamItem)
                }
                .CombineLatest(M =>
                {
                    var audioEnabled = M[0];
                    var audioOnlyMode = M[1];
                    var webcamMode = M[2];
                    var noWebcam = M[3];

                    if (audioOnlyMode)
                        return audioEnabled;

                    if (webcamMode)
                        return !noWebcam;

                    return true;
                })
                .ToReactiveCommand()
                .WithSubscribe(OnRecordExecute);

            PauseCommand = new[]
                {
                    TimerModel
                        .ObserveProperty(M => M.Waiting),
                    RecordingModel
                        .ObserveProperty(M => M.RecorderState)
                        .Select(M => M != Models.RecorderState.NotRecording)
                }
                .CombineLatest(M => !M[0] && M[1])
                .ToReactiveCommand()
                .WithSubscribe(() =>
                {
                    _audioPlayer.Play(SoundKind.Pause);

                    RecordingModel.OnPauseExecute();
                });

            RecorderState = RecordingModel
                .ObserveProperty(M => M.RecorderState)
                .ToReadOnlyReactivePropertySlim();
            
            TimerModel.DurationElapsed += async () =>
            {
                if (_syncContext != null)
                    _syncContext.Post(async State => await StopRecording(), null);
                else await StopRecording();
            };
        }

        async void OnRecordExecute()
        {
            if (RecorderState.Value == Models.RecorderState.NotRecording)
            {
                StartRecording();
            }
            else await StopRecording();
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

        async Task StopRecording()
        {
            _audioPlayer.Play(SoundKind.Stop);

            FileRecentItem savingRecentItem = null;
            FileSaveNotification notification = null;

            var fileName = _recordingModel.CurrentFileName;
            var isVideo = _recordingModel.IsVideo;

            // Assume saving to file only when extension is present
            if (!_timerModel.Waiting && !string.IsNullOrWhiteSpace(_videoWritersViewModel.SelectedVideoWriter.Extension))
            {
                savingRecentItem = new FileRecentItem(fileName, isVideo ? RecentFileType.Video : RecentFileType.Audio, true);
                _recentList.Add(savingRecentItem);

                notification = new FileSaveNotification(savingRecentItem);

                notification.OnDelete += () => savingRecentItem.RemoveCommand.ExecuteIfCan();

                _systemTray.ShowNotification(notification);
            }

            var task = _recordingModel.StopRecording();

            lock (_stopRecTaskLock)
            {
                _stopRecTasks.Add(task);
            }

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

        void StartRecording()
        {
            _systemTray.HideNotification();

            if (_recordingModel.StartRecording(new RecordingModelParams
            {
                VideoSourceKind = _videoSourcesViewModel.SelectedVideoSourceKind,
                VideoWriterKind = _videoWritersViewModel.SelectedVideoWriterKind,
                VideoWriter = _videoWritersViewModel.SelectedVideoWriter,
                AudioItems = _audioSourceViewModel.AvailableRecordingSources
            }))
            {
                if (Settings.Tray.MinToTrayOnCaptureStart)
                    _mainWindow.IsVisible = false;

                _audioPlayer.Play(SoundKind.Start);
            }
            else _audioPlayer.Play(SoundKind.Error);
        }

        public IReadOnlyReactiveProperty<RecorderState> RecorderState { get; }

        public bool CanExit()
        {
            if (RecorderState.Value == Models.RecorderState.Recording)
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