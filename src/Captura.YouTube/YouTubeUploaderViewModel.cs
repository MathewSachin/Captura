using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Captura.Models;
using Google.Apis.Upload;

namespace Captura.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class YouTubeUploaderViewModel : NotifyPropertyChanged
    {
        readonly YouTubeUploader _uploader;
        readonly CancellationTokenSource _cancellationTokenSource;
        readonly IMessageProvider _messageProvider;
        Task _uploadTask;
        string _fileName, _link, _title;
        string _description = "\n\n\n\n--------------------------------------------------\n\nUploaded using Captura (https://mathewsachin.github.io/Captura/)";
        YouTubePrivacyStatus _privacyStatus;
        int _progress;
        bool _beganUploading;
        YouTubeUploadRequest _uploadRequest;

        public YouTubeUploaderViewModel(YouTubeUploader Uploader,
            IMessageProvider MessageProvider,
            IClipboardService ClipboardService)
        {
            _uploader = Uploader;
            _messageProvider = MessageProvider;
            _cancellationTokenSource = new CancellationTokenSource();

            UploadCommand = new DelegateCommand(() => _uploadTask = OnUpload(), false);

            OpenVideoCommand = new DelegateCommand(() => Process.Start(Link), false);

            CopyLinkCommand = new DelegateCommand(() => ClipboardService.SetText(Link), false);
        }

        public string FileName
        {
            get => _fileName;
            private set
            {
                _fileName = value;

                OnPropertyChanged();
            }
        }

        public string Link
        {
            get => _link;
            private set
            {
                _link = value;

                var canExecute = !string.IsNullOrWhiteSpace(value);

                OpenVideoCommand.RaiseCanExecuteChanged(canExecute);
                CopyLinkCommand.RaiseCanExecuteChanged(canExecute);

                OnPropertyChanged();
            }
        }

        public async Task Init(string FilePath)
        {
            FileName = FilePath;
            Title = Path.GetFileName(FileName);

            var fileSize = new FileInfo(FileName).Length;

            _uploadRequest = await _uploader.CreateUploadRequest(FileName,
                Title,
                Description,
                PrivacyStatus: PrivacyStatus);

            _uploadRequest.Uploaded += L =>
            {
                Link = L;

                CancelBtnText = "Finish";
            };

            _uploadRequest.BytesSent += B => Progress = (int)(B * 100 / fileSize);
        }

        async Task OnUpload()
        {
            UploadCommand.RaiseCanExecuteChanged(false);

            var token = _cancellationTokenSource.Token;

            var task = BeganUploading
                ? _uploadRequest.Resume(token)
                : _uploadRequest.Upload(token);

            BeganUploading = true;

            try
            {
                var result = await task;

                if (result.Status == UploadStatus.Failed)
                {
                    _messageProvider.ShowException(result.Exception, "Error Occured while Uploading");

                    UploadCommand.RaiseCanExecuteChanged(true);

                    UploadBtnText = "Retry";
                }
            }
            catch (TaskCanceledException)
            {
                // Cancelled by user
            }
        }

        public string Title
        {
            get => _title;
            set
            {
                _title = value;

                UploadCommand.RaiseCanExecuteChanged(!string.IsNullOrWhiteSpace(value));

                OnPropertyChanged();
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;

                OnPropertyChanged();
            }
        }

        public YouTubePrivacyStatus PrivacyStatus
        {
            get => _privacyStatus;
            set
            {
                _privacyStatus = value;

                OnPropertyChanged();
            }
        }

        public IEnumerable<YouTubePrivacyStatus> PrivacyStatuses { get; } = new[]
        {
            YouTubePrivacyStatus.Public,
            YouTubePrivacyStatus.Unlisted,
            YouTubePrivacyStatus.Private
        };

        public async Task<bool> Cancel()
        {
            // Began Uploading but not finished
            if (BeganUploading && Link == null)
            {
                if (!_messageProvider.ShowYesNo("Are you sure you want to cancel upload?", "Cancel Upload"))
                    return false;
            }

            using (_cancellationTokenSource)
            using (_uploadRequest)
            {
                _cancellationTokenSource.Cancel();

                if (_uploadTask != null)
                    await _uploadTask;
            }

            return true;
        }

        public int Progress
        {
            get => _progress;
            private set
            {
                _progress = value;

                OnPropertyChanged();
            }
        }

        public bool BeganUploading
        {
            get => _beganUploading;
            private set
            {
                _beganUploading = value;

                OnPropertyChanged();
            }
        }

        public DelegateCommand UploadCommand { get; }

        public DelegateCommand OpenVideoCommand { get; }

        public DelegateCommand CopyLinkCommand { get; }

        string _uploadBtnText = "Upload", _cancelBtnText = "Cancel";

        public string UploadBtnText
        {
            get => _uploadBtnText;
            private set
            {
                _uploadBtnText = value;

                OnPropertyChanged();
            }
        }

        public string CancelBtnText
        {
            get => _cancelBtnText;
            private set
            {
                _cancelBtnText = value;

                OnPropertyChanged();
            }
        }
    }
}