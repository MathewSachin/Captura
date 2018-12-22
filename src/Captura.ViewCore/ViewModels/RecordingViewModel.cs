using System.Reactive.Linq;
using System.Windows.Input;
using Captura.Models;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Captura.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class RecordingViewModel : NotifyPropertyChanged
    {
        readonly RecordingModel _recordingModel;

        public ICommand RecordCommand { get; }
        public ICommand PauseCommand { get; }

        public RecordingViewModel(RecordingModel RecordingModel,
            Settings Settings,
            TimerModel TimerModel,
            VideoSourcesViewModel VideoSourcesViewModel)
        {
            _recordingModel = RecordingModel;

            RecordCommand = new[]
                {
                    Settings.Audio
                        .ObserveProperty(M => M.Enabled),
                    VideoSourcesViewModel
                        .ObserveProperty(M => M.SelectedVideoSourceKind)
                        .Select(M => !(M is NoVideoSourceProvider))
                }
                .CombineLatest(M => M[0] || M[1])
                .ToReactiveCommand()
                .WithSubscribe(RecordingModel.OnRecordExecute);

            PauseCommand = new[]
                {
                    TimerModel
                        .ObserveProperty(M => M.Waiting),
                    RecordingModel
                        .ObserveProperty(M => M.RecorderState)
                        .Select(M => M != RecorderState.NotRecording)
                }
                .CombineLatest(M => !M[0] && M[1])
                .ToReactiveCommand()
                .WithSubscribe(RecordingModel.OnPauseExecute);

            RecordingModel.PropertyChanged += (S, E) =>
            {
                switch (E.PropertyName)
                {
                    case "":
                    case null:
                    case nameof(RecorderState):
                        RaisePropertyChanged(nameof(RecorderState));
                        break;
                }
            };
        }

        public RecorderState RecorderState => _recordingModel.RecorderState;
    }
}