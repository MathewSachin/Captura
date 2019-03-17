using System;
using System.Diagnostics;
using System.IO;
using System.Reactive.Linq;
using System.Windows.Input;
using Captura.FFmpeg;
using Captura.Models;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Captura.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class FFmpegDownloadViewModel : NotifyPropertyChanged
    {
        public ICommand StartCommand { get; }
        public ICommand CancelCommand { get; }

        public ICommand SelectFolderCommand { get; }
        public ICommand OpenFolderCommand { get; }

        public IReadOnlyReactiveProperty<string> Status { get; }
        public IReadOnlyReactiveProperty<bool> InProgress { get; }
        public IReadOnlyReactiveProperty<bool> IsDone { get; }

        public FFmpegSettings FFmpegSettings { get; }

        public FFmpegDownloadViewModel(FFmpegSettings FFmpegSettings,
            FFmpegDownloadModel DownloadModel,
            IFFmpegViewsProvider FFmpegViewsProvider)
        {
            this.FFmpegSettings = FFmpegSettings;

            StartCommand = DownloadModel
                .ObserveProperty(M => M.State)
                .Select(M => M == FFmpegDownloaderState.Ready)
                .ToReactiveCommand()
                .WithSubscribe(async () =>
                {
                    var result = await DownloadModel.Start(M => Progress = M);

                    AfterDownload?.Invoke(result);
                });

            CancelCommand = DownloadModel
                .ObserveProperty(M => M.State)
                .Select(M => M == FFmpegDownloaderState.Downloading)
                .ToReactiveCommand()
                .WithSubscribe(() =>
                {
                    DownloadModel.Cancel();

                    CloseWindowAction?.Invoke();
                });

            SelectFolderCommand = DownloadModel
                .ObserveProperty(M => M.State)
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

            Status = new[]
                {
                    DownloadModel.ObserveProperty(M => M.State),
                    DownloadModel.ObserveProperty(M => M.Error)
                        .Select(M => FFmpegDownloaderState.Ready), // Dummy
                    this.ObserveProperty(M => M.Progress)
                        .Select(M => FFmpegDownloaderState.Ready) // Dummy
                }
                .CombineLatest(M =>
                {
                    var state = M[0];

                    switch (state)
                    {
                        case FFmpegDownloaderState.Downloading:
                            return $"{FFmpegDownloaderState.Downloading} ({Progress}%)";

                        case FFmpegDownloaderState.Error:
                            return DownloadModel.Error;

                        default:
                            return state.ToString();
                    }
                })
                .ToReadOnlyReactivePropertySlim();

            InProgress = DownloadModel
                .ObserveProperty(M => M.State)
                .Select(M => M == FFmpegDownloaderState.Downloading || M == FFmpegDownloaderState.Extracting)
                .ToReadOnlyReactivePropertySlim();

            IsDone = DownloadModel
                .ObserveProperty(M => M.State)
                .Select(M => M == FFmpegDownloaderState.Done || M == FFmpegDownloaderState.Cancelled || M == FFmpegDownloaderState.Error)
                .ToReadOnlyReactivePropertySlim();
        }

        public Action CloseWindowAction;
        public event Action<int> ProgressChanged;
        public event Action<bool> AfterDownload;

        int _progress;

        public int Progress
        {
            get => _progress;
            private set
            {
                if (!Set(ref _progress, value))
                    return;

                ProgressChanged?.Invoke(value);
            }
        }
    }
}
