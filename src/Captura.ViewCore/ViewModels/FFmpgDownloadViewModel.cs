using System;
using System.Diagnostics;
using System.IO;
using System.Reactive.Linq;
using System.Windows.Input;
using Captura.FFmpeg;
using Reactive.Bindings;

namespace Captura.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class FFmpegDownloadViewModel : NotifyPropertyChanged
    {
        public ICommand StartCommand { get; }
        public ICommand CancelCommand { get; }

        public ICommand SelectFolderCommand { get; }
        public ICommand OpenFolderCommand { get; }

        public IReadOnlyReactiveProperty<int> Progress { get; }
        public IReadOnlyReactiveProperty<string> Status { get; }
        public IReadOnlyReactiveProperty<bool> InProgress { get; }
        public IReadOnlyReactiveProperty<bool> IsDone { get; }

        public FFmpegSettings FFmpegSettings { get; }

        readonly IReactiveProperty<FFmpegDownloaderProgress> _downloaderProgress
            = new ReactivePropertySlim<FFmpegDownloaderProgress>(
                new FFmpegDownloaderProgress(FFmpegDownloaderState.Ready));

        public FFmpegDownloadViewModel(FFmpegSettings FFmpegSettings,
            FFmpegDownloadModel DownloadModel,
            IFFmpegViewsProvider FFmpegViewsProvider)
        {
            this.FFmpegSettings = FFmpegSettings;

            StartCommand = _downloaderProgress
                .Select(M => M.State)
                .Select(M => M == FFmpegDownloaderState.Ready)
                .ToReactiveCommand()
                .WithSubscribe(async () =>
                {
                    var progress = new Progress<FFmpegDownloaderProgress>(M => _downloaderProgress.Value = M);

                    var result = await DownloadModel.Start(progress);

                    AfterDownload?.Invoke(result);
                });

            CancelCommand = _downloaderProgress
                .Select(M => M.State)
                .Select(M => M == FFmpegDownloaderState.Downloading)
                .ToReactiveCommand()
                .WithSubscribe(() =>
                {
                    DownloadModel.Cancel();

                    CloseWindowAction?.Invoke();
                });

            SelectFolderCommand = _downloaderProgress
                .Select(M => M.State)
                .Select(M => M == FFmpegDownloaderState.Ready)
                .ToReactiveCommand()
                .WithSubscribe(FFmpegViewsProvider.PickFolder);

            OpenFolderCommand = new DelegateCommand(() =>
            {
                var path = FFmpegSettings.GetFolderPath();

                if (Directory.Exists(path))
                {
                    Process.Start(path);
                }
            });

            Status = _downloaderProgress
                .Select(M =>
                {
                    switch (M.State)
                    {
                        case FFmpegDownloaderState.Error:
                            return M.ErrorMessage;

                        case FFmpegDownloaderState.Downloading:
                            return $"{FFmpegDownloaderState.Downloading} ({M.DownloadProgress}%)";

                        default:
                            return M.State.ToString();
                    }
                })
                .ToReadOnlyReactivePropertySlim();

            Progress = _downloaderProgress
                .Where(M => M.State == FFmpegDownloaderState.Downloading)
                .Select(M => M.DownloadProgress)
                .ToReadOnlyReactivePropertySlim();

            Progress.Subscribe(M => ProgressChanged?.Invoke(M));

            InProgress = _downloaderProgress
                .Select(M => M.State)
                .Select(M => M == FFmpegDownloaderState.Downloading || M == FFmpegDownloaderState.Extracting)
                .ToReadOnlyReactivePropertySlim();

            IsDone = _downloaderProgress
                .Select(M => M.State)
                .Select(M => M == FFmpegDownloaderState.Done || M == FFmpegDownloaderState.Cancelled || M == FFmpegDownloaderState.Error)
                .ToReadOnlyReactivePropertySlim();
        }

        public Action CloseWindowAction;
        public event Action<int> ProgressChanged;
        public event Action<bool> AfterDownload;
    }
}
