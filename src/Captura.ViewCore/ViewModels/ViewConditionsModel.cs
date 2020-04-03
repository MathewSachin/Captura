using System.Reactive.Linq;
using System.Windows;
using Captura.FFmpeg;
using Captura.Models;
using Captura.Video;
using Captura.Webcam;
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

            IsStepsMode = Settings
                .Video
                .ObserveProperty(M => M.RecorderMode)
                .Select(M => M == RecorderMode.Steps)
                .ToReadOnlyReactivePropertySlim();

            IsNotAudioOrStepsMode = new[]
                {
                    VideoSourcesViewModel
                        .ObserveProperty(M => M.SelectedVideoSourceKind)
                        .Select(M => M is NoVideoSourceProvider),
                    IsStepsMode
                }
                .CombineLatest(M =>
                {
                    var audioMode = M[0];
                    var stepsMode = M[1];

                    return !audioMode && !stepsMode;
                })
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

            IsWebcamMode = VideoSourcesViewModel
                .ObserveProperty(M => M.SelectedVideoSourceKind)
                .Select(M => M is WebcamSourceProvider)
                .ToReadOnlyReactivePropertySlim();

            ShowSourceNameBox = VideoSourcesViewModel
                .ObserveProperty(M => M.SelectedVideoSourceKind)
                .Select(M => M is RegionSourceProvider || M is AroundMouseSourceProvider)
                .Select(M => !M)
                .ToReadOnlyReactivePropertySlim();

            StepsBtnEnabled = new[]
                {
                    IsEnabled,
                    VideoSourcesViewModel
                        .ObserveProperty(M => M.SelectedVideoSourceKind)
                        .Select(M => M.SupportsStepsMode)
                }
                .CombineLatestValuesAreAllTrue()
                .ToReadOnlyReactivePropertySlim();

            FpsVisibility = RecordingModel.ObserveProperty(M => M.RecorderState)
                .CombineLatest(IsNotAudioOrStepsMode,
                    (RecorderState, IsNotAudioOrStepsMode) => RecorderState == RecorderState.Recording && IsNotAudioOrStepsMode)
                .Select(M => M ? Visibility.Visible : Visibility.Hidden)
                .ToReadOnlyReactivePropertySlim();
        }

        public IReadOnlyReactiveProperty<bool> StepsBtnEnabled { get; }

        public IReadOnlyReactiveProperty<bool> IsNotAudioOrStepsMode { get; }

        public IReadOnlyReactiveProperty<bool> IsRegionMode { get; }

        public IReadOnlyReactiveProperty<bool> IsAudioMode { get; }

        public IReadOnlyReactiveProperty<bool> IsStepsMode { get; }

        public IReadOnlyReactiveProperty<bool> IsWebcamMode { get; }

        public IReadOnlyReactiveProperty<bool> MultipleVideoWriters { get; }

        public IReadOnlyReactiveProperty<bool> IsFFmpeg { get; }

        public IReadOnlyReactiveProperty<bool> IsVideoQuality { get; }

        public IReadOnlyReactiveProperty<bool> CanChangeWebcam { get; }

        public IReadOnlyReactiveProperty<bool> IsEnabled { get; }

        public IReadOnlyReactiveProperty<bool> CanWebcamSeparateFile { get; }

        public IReadOnlyReactiveProperty<bool> IsAroundMouseMode { get; }

        public IReadOnlyReactiveProperty<bool> IsReplayMode { get; }

        public IReadOnlyReactiveProperty<bool> ShowSourceNameBox { get; }

        public IReadOnlyReactiveProperty<Visibility> FpsVisibility { get; }
    }
}