using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Upload;
using Screna;

namespace Captura.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class YouTubeUploaderViewModel : NotifyPropertyChanged
    {
        readonly YouTubeUploader _uploader;
        readonly CancellationTokenSource _cancellationTokenSource;
        Task _uploadTask;

        public YouTubeUploaderViewModel(YouTubeUploader Uploader)
        {
            _uploader = Uploader;
            _cancellationTokenSource = new CancellationTokenSource();

            UploadCommand = new DelegateCommand(() => _uploadTask = OnUpload(), false);

            OpenVideoCommand = new DelegateCommand(() => Process.Start(Link), false);

            CopyLinkCommand = new DelegateCommand(() => Link.WriteToClipboard(), false);
        }

        string _fileName;

        public string FileName
        {
            get => _fileName;
            set
            {
                _fileName = value;

                OnPropertyChanged();
            }
        }

        string _link;

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

        async Task OnUpload()
        {
            UploadCommand.RaiseCanExecuteChanged(false);

            var fileSize = new FileInfo(FileName).Length;

            var uploadRequest = await _uploader.CreateUploadRequest(FileName,
                Title,
                Description,
                PrivacyStatus: PrivacyStatus);

            uploadRequest.Uploaded += L => Link = L;

            uploadRequest.BytesSent += B => Progress = (int)(B * 100 / fileSize);

            Uploading = true;

            var result = await uploadRequest.Upload(_cancellationTokenSource.Token);

            if (result.Status == UploadStatus.Failed)
            {
                ServiceProvider.MessageProvider.ShowException(result.Exception, "Error Occured while Uploading");
            }

            Uploading = false;
        }

        string _title;

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

        string _description = "\n\n\n\n--------------------------------------------------\n\nUploaded using Captura (https://mathewsachin.github.io/Captura/)";

        public string Description
        {
            get => _description;
            set
            {
                _description = value;

                OnPropertyChanged();
            }
        }

        YouTubePrivacyStatus _privacyStatus;

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

        public void Cancel()
        {
            _cancellationTokenSource.Cancel();

            _uploadTask?.Wait();

            _cancellationTokenSource.Dispose();
        }

        int _progress;

        public int Progress
        {
            get => _progress;
            set
            {
                _progress = value;

                OnPropertyChanged();
            }
        }

        public DelegateCommand UploadCommand { get; }

        public DelegateCommand OpenVideoCommand { get; }

        public DelegateCommand CopyLinkCommand { get; }

        bool _uploading;

        public bool Uploading
        {
            get => _uploading;
            set
            {
                _uploading = value;

                OnPropertyChanged();
            }
        }
    }
}