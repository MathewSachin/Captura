using System.Collections.Generic;
using System.IO;

namespace Captura.ViewModels
{
    public class YouTubeUploaderViewModel : NotifyPropertyChanged
    {
        public YouTubeUploaderViewModel()
        {
            UploadCommand = new DelegateCommand(OnUpload, false);
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

                OnPropertyChanged();
            }
        }

        async void OnUpload()
        {
            UploadCommand.RaiseCanExecuteChanged(false);

            var uploader = new YouTubeUploader();

            var fileSize = new FileInfo(FileName).Length;

            uploader.Uploaded += L => Link = L;
            uploader.ErrorOccured += E => ServiceProvider.MessageProvider.ShowException(E, "Error Occured while Uploading");

            uploader.BytesSent += B => Progress = (int)(B * 100 / fileSize);

            await uploader.Upload(FileName,
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

        string _description = "\n\n\n\n--------------------------------------------------\n\nUploaded using Captura";

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
    }
}