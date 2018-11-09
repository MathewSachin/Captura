using System.Collections.Generic;
using System.IO;
using Captura.Models;

namespace Captura.ViewModels
{
    public class YouTubeUploaderViewModel : NotifyPropertyChanged
    {
        readonly IMessageProvider _messageProvider;

        public YouTubeUploaderViewModel(IMessageProvider MessageProvider)
        {
            _messageProvider = MessageProvider;

            UploadCommand = new DelegateCommand(OnUpload, false);
        }

        public string FileName { get; set; }

        public string Link { get; private set; }

        async void OnUpload()
        {
            var uploader = new YouTubeUploader();

            var fileSize = new FileInfo(FileName).Length;

            uploader.Uploaded += L => Link = L;
            uploader.ErrorOccured += E => _messageProvider.ShowException(E, "Error Occured while Uploading");

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

        string _description;

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
            YouTubePrivacyStatus.Unlinsted,
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