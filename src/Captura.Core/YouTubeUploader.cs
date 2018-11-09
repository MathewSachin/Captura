using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace Captura
{
    public enum YouTubePrivacyStatus
    {
        Public,
        Unlisted,
        Private
    }

    public class YouTubeUploader
    {
        static string GetPrivacyStatus(YouTubePrivacyStatus PrivacyStatus)
        {
            return PrivacyStatus.ToString().ToLower();
        }

        public async Task Upload(string FileName,
            string Title,
            string Description,
            string[] Tags = null,
            YouTubePrivacyStatus PrivacyStatus = YouTubePrivacyStatus.Unlisted)
        {
            var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync
            (
                new ClientSecrets
                {
                    ClientId = ApiKeys.YouTubeClientId,
                    ClientSecret = ApiKeys.YouTubeClientSecret
                },
                // This OAuth 2.0 access scope allows an application to upload files to the
                // authenticated user's YouTube channel, but doesn't allow other types of access.
                new[] { YouTubeService.Scope.YoutubeUpload },
                "user",
                CancellationToken.None
            );

            var youtubeService = new YouTubeService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = nameof(Captura)
            });

            var video = new Video
            {
                Snippet = new VideoSnippet
                {
                    Title = Title,
                    Description = Description,
                    Tags = Tags ?? new string[0],
                    CategoryId = "22"
                },
                Status = new VideoStatus { PrivacyStatus = GetPrivacyStatus(PrivacyStatus) }
            };

            using (var fileStream = new FileStream(FileName, FileMode.Open))
            {
                var videosInsertRequest = youtubeService.Videos.Insert(video, "snippet,status", fileStream, "video/*");
                videosInsertRequest.ProgressChanged += VideosInsertRequest_ProgressChanged;
                videosInsertRequest.ResponseReceived += VideosInsertRequest_ResponseReceived;

                await videosInsertRequest.UploadAsync();
            }
        }

        void VideosInsertRequest_ProgressChanged(IUploadProgress Progress)
        {
            switch (Progress.Status)
            {
                case UploadStatus.Uploading:
                    BytesSent?.Invoke(Progress.BytesSent);
                    break;

                case UploadStatus.Failed:
                    ErrorOccured?.Invoke(Progress.Exception);
                    break;
            }
        }

        void VideosInsertRequest_ResponseReceived(Video Video)
        {
            Uploaded?.Invoke($"https://youtube.com/watch?v={Video.Id}");
        }

        public event Action<long> BytesSent;

        public event Action<Exception> ErrorOccured;

        public event Action<string> Uploaded;
    }
}