using System.Reactive.Linq;
using Captura.Models;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Captura.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ViewConditionsModel
    {
        public ViewConditionsModel(VideoSourcesViewModel VideoSourcesViewModel,
            VideoWritersViewModel VideoWritersViewModel,
            Settings Settings,
            RecordingModel RecordingModel,
            AudioSourceViewModel AudioSourceViewModel)
        {
            IsRegionMode = VideoSourcesViewModel
                .ObserveProperty(M => M.SelectedVideoSourceKind)
                .Select(M => M is RegionSourceProvider)
                .ToReadOnlyReactivePropertySlim();

            IsAudioMode = VideoSourcesViewModel
                .ObserveProperty(M => M.SelectedVideoSourceKind)
                .Select(M => M is NoVideoSourceProvider)
                .ToReadOnlyReactivePropertySlim();

            MultipleVideoWriters = VideoWritersViewModel.AvailableVideoWriters
                .ObserveProperty(M => M.Count)
                .Select(M => M > 1)
                .ToReadOnlyReactivePropertySlim();

            IsFFmpeg = VideoWritersViewModel
                .ObserveProperty(M => M.SelectedVideoWriterKind)
                .Select(M => M is FFmpegWriterProvider || M is StreamingWriterProvider)
                .ToReadOnlyReactivePropertySlim();

            IsVideoQuality = VideoWritersViewModel
                .ObserveProperty(M => M.SelectedVideoWriterKind)
                .Select(M => M is DiscardWriterProvider)
                .Select(M => !M)
                .ToReadOnlyReactivePropertySlim();

            IsReplayMode = Settings
                .Video
                .ObserveProperty(M => M.RecorderMode)
                .Select(M => M == RecorderMode.Replay)
                .ToReadOnlyReactivePropertySlim();

            CanChangeWebcam = new[]
                {
                    RecordingModel
                        .ObserveProperty(M => M.RecorderState)
                        .Select(M => M == RecorderState.NotRecording),
                    Settings.WebcamOverlay
                        .ObserveProperty(M => M.SeparateFile),
                    VideoSourcesViewModel
                        .ObserveProperty(M => M.SelectedVideoSourceKind)
                        .Select(M => M is WebcamSourceProvider)
                }
                .CombineLatest(M =>
                {
                    var notRecording = M[0];
                    var separateFile = M[1];
                    var webcamMode = M[2];

                    if (webcamMode)
                    {
                        return notRecording;
                    }

                    return !separateFile || notRecording;
                })
                .ToReadOnlyReactivePropertySlim();

            CanChangeAudioSources = new[]
                {
                    RecordingModel
                        .ObserveProperty(M => M.RecorderState)
                        .Select(M => M == RecorderState.NotRecording),
                    Settings.Audio
                        .ObserveProperty(M => M.SeparateFilePerSource)
                }
                .CombineLatest(M =>
                {
                    var notRecording = M[0];
                    var separateFilePerSource = M[1];

                    if (notRecording)
                        return true;

                    return !separateFilePerSource && AudioSourceViewModel.CanChangeSourcesDuringRecording;
                })
                .ToReadOnlyReactivePropertySlim();

            IsEnabled = RecordingModel
                .ObserveProperty(M => M.RecorderState)
                .Select(M => M == RecorderState.NotRecording)
                .ToReadOnlyReactivePropertySlim();

            CanWebcamSeparateFile = VideoSourcesViewModel
                .ObserveProperty(M => M.SelectedVideoSourceKind)
                .Select(M => M is WebcamSourceProvider)
                .Select(M => !M)
                .ToReadOnlyReactivePropertySlim();

            IsAroundMouseMode = VideoSourcesViewModel
                .ObserveProperty(M => M.SelectedVideoSourceKind)
                .Select(M => M is AroundMouseSourceProvider)
                .ToReadOnlyReactivePropertySlim();

            ShowSourceNameBox = VideoSourcesViewModel
                .ObserveProperty(M => M.SelectedVideoSourceKind)
                .Select(M => M is RegionSourceProvider || M is AroundMouseSourceProvider)
                .Select(M => !M)
                .ToReadOnlyReactivePropertySlim();
        }

        public IReadOnlyReactiveProperty<bool> IsRegionMode { get; }

        public IReadOnlyReactiveProperty<bool> IsAudioMode { get; }

        public IReadOnlyReactiveProperty<bool> MultipleVideoWriters { get; }

        public IReadOnlyReactiveProperty<bool> IsFFmpeg { get; }

        public IReadOnlyReactiveProperty<bool> IsVideoQuality { get; }

        public IReadOnlyReactiveProperty<bool> CanChangeWebcam { get; }

        public IReadOnlyReactiveProperty<bool> CanChangeAudioSources { get; }

        public IReadOnlyReactiveProperty<bool> IsEnabled { get; }

        public IReadOnlyReactiveProperty<bool> CanWebcamSeparateFile { get; }

        public IReadOnlyReactiveProperty<bool> IsAroundMouseMode { get; }

        public IReadOnlyReactiveProperty<bool> IsReplayMode { get; }

        public IReadOnlyReactiveProperty<bool> ShowSourceNameBox { get; }
    }
}