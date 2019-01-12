using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Screna;

namespace Captura.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class YouTubeUploaderViewModel : NotifyPropertyChanged
    {
        readonly YouTubeUploader _uploader;

        public YouTubeUploaderViewModel(YouTubeUploader Uploader)
        {
            _uploader = Uploader;
            UploadCommand = new DelegateCommand(OnUpload, false);

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

        async void OnUpload()
        {
            UploadCommand.RaiseCanExecuteChanged(false);

            var fileSize = new FileInfo(FileName).Length;

            _uploader.Uploaded += L =>
            {
                Uploading = false;

                Progress = 100;

                Link = L;
            };

            _uploader.ErrorOccured += E =>
            {
                Uploading = false;

                ServiceProvider.MessageProvider.ShowException(E, "Error Occured while Uploading");
            };

            _uploader.BytesSent += B => Progress = (int)(B * 100 / fileSize);

            Uploading = true;

            await _uploader.Upload(FileName,
                Title,
                Description,
                PrivacyStatus: PrivacyStatus);
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