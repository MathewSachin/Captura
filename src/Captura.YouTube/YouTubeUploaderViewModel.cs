using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Captura.Models;
using Google.Apis.Upload;
using Screna;

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
            IMessageProvider MessageProvider)
        {
            _uploader = Uploader;
            _messageProvider = MessageProvider;
            _cancellationTokenSource = new CancellationTokenSource();

            UploadCommand = new DelegateCommand(() => _uploadTask = OnUpload(), false);

            OpenVideoCommand = new DelegateCommand(() => Process.Start(Link), false);

            CopyLinkCommand = new DelegateCommand(() => Link.WriteToClipboard(), false);
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

            _uploadRequest.Uploaded += L => Link = L;

            _uploadRequest.BytesSent += B => Progress = (int)(B * 100 / fileSize);
        }

        async Task OnUpload()
        {
            UploadCommand.RaiseCanExecuteChanged(false);

            BeganUploading = true;

            var result = await _uploadRequest.Upload(_cancellationTokenSource.Token);

            if (result.Status == UploadStatus.Failed)
            {
                ServiceProvider.MessageProvider.ShowException(result.Exception, "Error Occured while Uploading");
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

        public bool Cancel()
        {
            if (_uploadTask != null)
            {
                if (!_messageProvider.ShowYesNo("Are you sure you want to cancel upload?", "Cancel Upload"))
                    return false;
            }

            _cancellationTokenSource.Cancel();

            _uploadTask?.Wait();

            _cancellationTokenSource.Dispose();

            return true;
        }

        public int Progress
        {
            get => _progress;
            set
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
    }
}