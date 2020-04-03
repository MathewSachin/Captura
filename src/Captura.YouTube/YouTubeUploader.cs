using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace Captura.YouTube
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class YouTubeUploader
    {
        readonly IYouTubeApiKeys _apiKeys;
        readonly ProxySettings _proxySettings;

        YouTubeService _youtubeService;

        public YouTubeUploader(IYouTubeApiKeys ApiKeys,
            ProxySettings ProxySettings)
        {
            _apiKeys = ApiKeys;
            _proxySettings = ProxySettings;
        }

        static string GetPrivacyStatus(YouTubePrivacyStatus PrivacyStatus)
        {
            return PrivacyStatus.ToString().ToLower();
        }

        public async Task<YouTubeUploadRequest> CreateUploadRequest(string FileName,
            string Title,
            string Description,
            string[] Tags = null,
            YouTubePrivacyStatus PrivacyStatus = YouTubePrivacyStatus.Unlisted)
        {
            if (_youtubeService == null)
                await Init();

            var video = new Google.Apis.YouTube.v3.Data.Video
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

            return new YouTubeUploadRequest(FileName, _youtubeService, video);
        }

        async Task Init()
        {
            WebRequest.DefaultWebProxy = _proxySettings.GetWebProxy();

            var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync
            (
                new ClientSecrets
                {
                    ClientId = _apiKeys.YouTubeClientId,
                    ClientSecret = _apiKeys.YouTubeClientSecret
                },
                // This OAuth 2.0 access scope allows an application to upload files to the
                // authenticated user's YouTube channel, but doesn't allow other types of access.
                new[] { YouTubeService.Scope.YoutubeUpload },
                "user",
                CancellationToken.None
            );

            _youtubeService = new YouTubeService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = nameof(Captura)
            });
        }
    }
}