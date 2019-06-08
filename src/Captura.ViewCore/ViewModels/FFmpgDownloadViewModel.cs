using System;
using System.Diagnostics;
using System.IO;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Captura.FFmpeg;
using Captura.Models;
using Reactive.Bindings;

namespace Captura.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class FFmpegDownloadViewModel : NotifyPropertyChanged
    {
        public ICommand StartCommand { get; }

        public ICommand SelectFolderCommand { get; }
        public ICommand OpenFolderCommand { get; }

        public IReadOnlyReactiveProperty<int> Progress { get; }
        public IReadOnlyReactiveProperty<string> Status { get; }
        public IReadOnlyReactiveProperty<bool> InProgress { get; }
        public IReadOnlyReactiveProperty<bool> IsDone { get; }
        public IReadOnlyReactiveProperty<bool> CanCancel { get; }

        public FFmpegSettings FFmpegSettings { get; }

        readonly IReactiveProperty<FFmpegDownloaderProgress> _downloaderProgress
            = new ReactivePropertySlim<FFmpegDownloaderProgress>(
                new FFmpegDownloaderProgress(FFmpegDownloaderState.Ready));

        readonly IMessageProvider _messageProvider;
        readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        Task<bool> _downloadTask;

        public FFmpegDownloadViewModel(FFmpegSettings FFmpegSettings,
            FFmpegDownloadModel DownloadModel,
            IFFmpegViewsProvider FFmpegViewsProvider,
            IMessageProvider MessageProvider)
        {
            this.FFmpegSettings = FFmpegSettings;
            _messageProvider = MessageProvider;

            StartCommand = _downloaderProgress
                .Select(M => M.State)
                .Select(M => M == FFmpegDownloaderState.Ready)
                .ToReactiveCommand()
                .WithSubscribe(async () =>
                {
                    var progress = new Progress<FFmpegDownloaderProgress>(M => _downloaderProgress.Value = M);

                    _downloadTask = DownloadModel.Start(progress, _cancellationTokenSource.Token);

                    var result = await _downloadTask;

                    AfterDownload?.Invoke(result);
                });

            CanCancel = _downloaderProgress
                .Select(M => M.State)
                .Select(M => M == FFmpegDownloaderState.Downloading)
                .ToReadOnlyReactivePropertySlim();

            SelectFolderCommand = _downloaderProgress
                .Select(M => M.State)
                .Select(M => M == FFmpegDownloaderState.Ready)
                .ToReactiveCommand()
                .WithSubscribe(FFmpegViewsProvider.PickFolder);

            OpenFolderCommand = new ReactiveCommand()
                .WithSubscribe(() =>
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

        public event Action<int> ProgressChanged;
        public event Action<bool> AfterDownload;

        public async Task<bool> Cancel()
        {
            if (CanCancel.Value)
            {
                if (!_messageProvider.ShowYesNo("Are you sure you want to cancel download?", "Cancel Download"))
                    return false;

                _cancellationTokenSource.Cancel();
            }
            else if (InProgress.Value)
            {
                return false;
            }

            _cancellationTokenSource.Dispose();

            if (_downloadTask != null)
            {
                await _downloadTask;
            }

            return true;
        }
    }
}
