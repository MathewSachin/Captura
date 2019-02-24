using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Captura.Models;

namespace Captura.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class FFmpegDownloadViewModel : NotifyPropertyChanged
    {
        public DelegateCommand StartCommand { get; }

        public DelegateCommand SelectFolderCommand { get; }

        public ICommand OpenFolderCommand { get; }

        readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        bool _isDownloading;

        public bool IsDownloading
        {
            get => _isDownloading;
            set
            {
                _isDownloading = value;
                
                OnPropertyChanged();
            }
        }

        readonly IDialogService _dialogService;
        readonly ProxySettings _proxySettings;
        public FFmpegSettings FFmpegSettings { get; }
        readonly LanguageManager _loc;

        public FFmpegDownloadViewModel(IDialogService DialogService,
            ProxySettings ProxySettings,
            LanguageManager Loc,
            FFmpegSettings FFmpegSettings)
        {
            _dialogService = DialogService;
            _proxySettings = ProxySettings;
            _loc = Loc;

            this.FFmpegSettings = FFmpegSettings;

            StartCommand = new DelegateCommand(OnStartExecute);

            SelectFolderCommand = new DelegateCommand(OnSelectFolderExecute);

            OpenFolderCommand = new DelegateCommand(() =>
            {
                var path = FFmpegSettings.GetFolderPath();

                if (Directory.Exists(path))
                {
                    Process.Start(path);
                }
            });

            EnsureDir();
        }

        void EnsureDir()
        {
            var path = FFmpegSettings.GetFolderPath();

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        async void OnStartExecute()
        {
            IsDownloading = true;

            try
            {
                var result = await Start();

                AfterDownload?.Invoke(result);
            }
            finally
            {
                IsDownloading = false;
            }
        }

        void OnSelectFolderExecute()
        {
            var folder = _dialogService.PickFolder(FFmpegSettings.GetFolderPath(), _loc.SelectFFmpegFolder);

            if (!string.IsNullOrWhiteSpace(folder))
                FFmpegSettings.FolderPath = folder;
        }

        const string CancelDownload = "Cancel Download";
        const string StartDownload = "Start Download";
        const string Finish = "Finish";

        public Action CloseWindowAction;

        public event Action<int> ProgressChanged;

        public event Action<bool> AfterDownload;
        
        public async Task<bool> Start()
        {
            switch (ActionDescription)
            {
                case CancelDownload:
                    _cancellationTokenSource.Cancel();

                    CloseWindowAction.Invoke();
                
                    return false;

                case Finish:
                    CloseWindowAction?.Invoke();

                    return true;
            }

            ActionDescription = CancelDownload;

            Status = "Downloading";

            try
            {
                await DownloadFFmpeg.DownloadArchive(P =>
                {
                    Progress = P;

                    Status = $"Downloading ({P}%)";

                    ProgressChanged?.Invoke(P);
                }, _proxySettings.GetWebProxy(), _cancellationTokenSource.Token);
            }
            catch (WebException webException) when(webException.Status == WebExceptionStatus.RequestCanceled)
            {
                Status = "Cancelled";
                return false;
            }
            catch (Exception e)
            {
                Status = $"Failed - {e.Message}";
                return false;
            }

            _cancellationTokenSource.Dispose();

            // No cancelling after download
            StartCommand.RaiseCanExecuteChanged(false);
            
            Status = "Extracting";

            try
            {
                await DownloadFFmpeg.ExtractTo(FFmpegSettings.GetFolderPath());
            }
            catch (UnauthorizedAccessException)
            {
                Status = "Can't extract to specified directory";
                return false;
            }
            catch
            {
                Status = "Extraction failed";
                return false;
            }
            
            Status = "Done";
            ActionDescription = Finish;

            StartCommand.RaiseCanExecuteChanged(true);

            return true;
        }

        string _actionDescription = StartDownload;

        public string ActionDescription
        {
            get => _actionDescription;
            private set
            {
                _actionDescription = value;

                OnPropertyChanged();
            }
        }

        int _progress;

        public int Progress
        {
            get => _progress;
            private set
            {
                _progress = value;

                OnPropertyChanged();
            }
        }

        string _status = "Ready";

        public string Status
        {
            get => _status;
            private set
            {
                _status = value;

                OnPropertyChanged();
            }
        }
    }
}
