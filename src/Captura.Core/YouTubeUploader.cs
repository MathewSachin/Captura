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
    public class YouTubeUploader
    {
        public async Task Upload(string FileName, string Title, string Description, string[] Tags = null)
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
                Status = new VideoStatus { PrivacyStatus = "unlisted" }
                // See https://developers.google.com/youtube/v3/docs/videoCategories/list
                // or "private" or "public"
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
                    Console.WriteLine("{0} bytes sent.", Progress.BytesSent);
                    break;

                case UploadStatus.Failed:
                    Console.WriteLine("An error prevented the upload from completing.\n{0}", Progress.Exception);
                    break;
            }
        }

        void VideosInsertRequest_ResponseReceived(Video Video)
        {
            Console.WriteLine("Video id '{0}' was successfully uploaded.", Video.Id);
        }
    }
}