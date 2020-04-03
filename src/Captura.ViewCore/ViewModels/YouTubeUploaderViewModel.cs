using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Captura.Models;
using Captura.YouTube;
using Google.Apis.Upload;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

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

        readonly IReactiveProperty<bool> _canUpload = new ReactivePropertySlim<bool>(true);

        public YouTubeUploaderViewModel(YouTubeUploader Uploader,
            IMessageProvider MessageProvider,
            IClipboardService ClipboardService)
        {
            _uploader = Uploader;
            _messageProvider = MessageProvider;
            _cancellationTokenSource = new CancellationTokenSource();

            OpenVideoCommand = this
                .ObserveProperty(M => M.Link)
                .Select(M => !string.IsNullOrWhiteSpace(M))
                .ToReactiveCommand()
                .WithSubscribe(() => Process.Start(Link));

            CopyLinkCommand = this
                .ObserveProperty(M => M.Link)
                .Select(M => !string.IsNullOrWhiteSpace(M))
                .ToReactiveCommand()
                .WithSubscribe(() => ClipboardService.SetText(Link));

            UploadCommand = new[]
                {
                    _canUpload,
                    this.ObserveProperty(M => Title)
                        .Select(M => !string.IsNullOrWhiteSpace(M))
                }
                .CombineLatestValuesAreAllTrue()
                .ToReactiveCommand()
                .WithSubscribe(() => _uploadTask = OnUpload());
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
            _canUpload.Value = false;

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

                    _canUpload.Value = true;

                    UploadBtnText = "Retry";
                }
            }
            catch (TaskCanceledException)
            {
                // Cancelled by user
            }
        }

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

        public string FileName
        {
            get => _fileName;
            private set => Set(ref _fileName, value);
        }

        public string Link
        {
            get => _link;
            private set => Set(ref _link, value);
        }

        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }

        public string Description
        {
            get => _description;
            set => Set(ref _description, value);
        }

        public YouTubePrivacyStatus PrivacyStatus
        {
            get => _privacyStatus;
            set => Set(ref _privacyStatus, value);
        }

        public IEnumerable<YouTubePrivacyStatus> PrivacyStatuses { get; } = new[]
        {
            YouTubePrivacyStatus.Public,
            YouTubePrivacyStatus.Unlisted,
            YouTubePrivacyStatus.Private
        };

        public int Progress
        {
            get => _progress;
            private set => Set(ref _progress, value);
        }

        public bool BeganUploading
        {
            get => _beganUploading;
            private set => Set(ref _beganUploading, value);
        }

        public ICommand UploadCommand { get; }
        public ICommand OpenVideoCommand { get; }
        public ICommand CopyLinkCommand { get; }

        string _uploadBtnText = "Upload", _cancelBtnText = "Cancel";

        public string UploadBtnText
        {
            get => _uploadBtnText;
            private set => Set(ref _uploadBtnText, value);
        }

        public string CancelBtnText
        {
            get => _cancelBtnText;
            private set => Set(ref _cancelBtnText, value);
        }
    }
}