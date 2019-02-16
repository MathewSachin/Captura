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
            AudioSource AudioSource)
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
                .Select(M => !(M is DiscardWriterProvider))
                .ToReadOnlyReactivePropertySlim();

            CanChangeWebcam = new[]
                {
                    RecordingModel
                        .ObserveProperty(M => M.RecorderState)
                        .Select(M => M == RecorderState.NotRecording),
                    Settings.WebcamOverlay
                        .ObserveProperty(M => M.SeparateFile)
                }
                .CombineLatest(M => !M[1] || M[0]) // Not SeparateFile or NotRecording
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
                    AudioSource.CanChangeSourcesDuringRecording ||
                    !M[1] || M[0]) // Not SeparateFilePerSource or NotRecording
                .ToReadOnlyReactivePropertySlim();

            IsEnabled = RecordingModel
                .ObserveProperty(M => M.RecorderState)
                .Select(M => M == RecorderState.NotRecording)
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
    }
}