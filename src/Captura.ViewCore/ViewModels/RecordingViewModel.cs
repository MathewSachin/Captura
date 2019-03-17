using System.Reactive.Linq;
using System.Windows.Input;
using Captura.Models;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Captura.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class RecordingViewModel : ViewModelBase
    {
        readonly RecordingModel _recordingModel;
        readonly ISystemTray _systemTray;
        readonly IMainWindow _mainWindow;
        readonly IAudioPlayer _audioPlayer;

        public ICommand RecordCommand { get; }
        public ICommand PauseCommand { get; }

        public RecordingViewModel(RecordingModel RecordingModel,
            Settings Settings,
            TimerModel TimerModel,
            WebcamModel WebcamModel,
            VideoSourcesViewModel VideoSourcesViewModel,
            ISystemTray SystemTray,
            IMainWindow MainWindow,
            ILocalizationProvider Loc,
            IAudioPlayer AudioPlayer) : base(Settings, Loc)
        {
            _recordingModel = RecordingModel;
            _systemTray = SystemTray;
            _mainWindow = MainWindow;
            _audioPlayer = AudioPlayer;

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
        }

        async void OnRecordExecute()
        {
            if (RecorderState.Value == Models.RecorderState.NotRecording)
            {
                _systemTray.HideNotification();

                if (_recordingModel.StartRecording())
                {
                    if (Settings.Tray.MinToTrayOnCaptureStart)
                        _mainWindow.IsVisible = false;

                    _audioPlayer.Play(SoundKind.Start);
                }
                else _audioPlayer.Play(SoundKind.Error);
            }
            else
            {
                _audioPlayer.Play(SoundKind.Stop);

                await _recordingModel.StopRecording();
            }
        }

        public IReadOnlyReactiveProperty<RecorderState> RecorderState { get; }
    }
}