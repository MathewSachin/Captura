using Ookii.Dialogs;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Captura.ViewModels
{
    public class FFMpegDownloadViewModel : ViewModelBase
    {
        public DelegateCommand StartCommand { get; }

        public DelegateCommand SelectFolderCommand { get; }

        readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public FFMpegDownloadViewModel(Settings Settings, LanguageManager LanguageManager) : base(Settings, LanguageManager)
        {
            StartCommand = new DelegateCommand(async () =>
            {
                var result = await Start();

                AfterDownload?.Invoke(result);
            });

            SelectFolderCommand = new DelegateCommand(() =>
            {
                using (var dlg = new VistaFolderBrowserDialog
                {
                    SelectedPath = TargetFolder,
                    UseDescriptionForTitle = true,
                    Description = LanguageManager.SelectFFMpegFolder
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
            if (ActionDescription == CancelDownload)
            {
                _cancellationTokenSource.Cancel();

                CloseWindowAction.Invoke();
                
                return false;
            }

            if (ActionDescription == Finish)
            {
                CloseWindowAction?.Invoke();

                return true;
            }

            ActionDescription = CancelDownload;

            Status = "Downloading";

            try
            {
                await DownloadFFMpeg.DownloadArchive(P =>
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
                await DownloadFFMpeg.ExtractTo(TargetFolder);
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
            
            // Update FFMpeg folder setting
            Settings.FFMpeg.FolderPath = TargetFolder;
            
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
