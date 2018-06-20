using Ookii.Dialogs;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Captura.ViewModels
{
    public class FFmpegDownloadViewModel : ViewModelBase
    {
        public DelegateCommand StartCommand { get; }

        public DelegateCommand SelectFolderCommand { get; }

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

        public FFmpegDownloadViewModel(Settings Settings, LanguageManager LanguageManager) : base(Settings, LanguageManager)
        {
            StartCommand = new DelegateCommand(async () =>
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
            });

            SelectFolderCommand = new DelegateCommand(() =>
            {
                using (var dlg = new VistaFolderBrowserDialog
                {
                    SelectedPath = TargetFolder,
                    UseDescriptionForTitle = true,
                    Description = LanguageManager.SelectFFmpegFolder
                })
                {
                    if (dlg.ShowDialog() == DialogResult.OK)
                        TargetFolder = dlg.SelectedPath;
                }
            });
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
                }, Settings.Proxy.GetWebProxy(), _cancellationTokenSource.Token);
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
                await DownloadFFmpeg.ExtractTo(TargetFolder);
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
            
            // Update FFmpeg folder setting
            Settings.FFmpeg.FolderPath = TargetFolder;
            
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

        string _targetFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public string TargetFolder
        {
            get => _targetFolder;
            set
            {
                _targetFolder = value;

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
