using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Captura.Models;
using Newtonsoft.Json;

namespace Captura.Imgur
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ImgurUploader : IImageUploader
    {
        readonly ImgurSettings _settings;
        readonly ProxySettings _proxySettings;
        readonly IImgurApiKeys _apiKeys;

        public ImgurUploader(ImgurSettings Settings,
            ProxySettings ProxySettings,
            IImgurApiKeys ApiKeys)
        {
            _settings = Settings;
            _proxySettings = ProxySettings;
            _apiKeys = ApiKeys;
        }

        public async Task<UploadResult> Upload(IBitmapImage Image, ImageFormats Format, Action<int> Progress)
        {
            using var w = new WebClient { Proxy = _proxySettings.GetWebProxy() };
            if (Progress != null)
            {
                w.UploadProgressChanged += (S, E) => Progress(E.ProgressPercentage);
            }

            w.Headers.Add("Authorization", await GetAuthorizationHeader());

            NameValueCollection values;

            using (var ms = new MemoryStream())
            {
                Image.Save(ms, Format);

                values = new NameValueCollection
                {
                    { "image", Convert.ToBase64String(ms.ToArray()) }
                };
            }

            var uploadResponse = await UploadValuesAsync<ImgurUploadResponse>(w, "https://api.imgur.com/3/upload.json", values);

            if (!uploadResponse.Success)
            {
                throw new Exception("Response indicates Failure");
            }

            return new UploadResult
            {
                Url = uploadResponse.Data.Link,
                DeleteLink = $"https://api.imgur.com/3/image/{uploadResponse.Data.DeleteHash}"
            };
        }

        async Task<string> GetAuthorizationHeader()
        {
            if (_settings.Anonymous)
            {
                return $"Client-ID {_apiKeys.ImgurClientId}";
            }

            if (string.IsNullOrWhiteSpace(_settings.AccessToken))
            {
                throw new Exception("Not logged in to Imgur");
            }

            if (_settings.IsExpired())
            {
                if (!await RefreshToken())
                {
                    throw new Exception("Failed to Refresh Imgur token");
                }
            }

            return $"Bearer {_settings.AccessToken}";
        }

        async Task<bool> RefreshToken()
        {
            var args = new NameValueCollection
            {
                { "refresh_token", _settings.RefreshToken },
                { "client_id", _apiKeys.ImgurClientId },
                { "client_secret", _apiKeys.ImgurSecret },
                { "grant_type", "refresh_token" }
            };

            using var w = new WebClient { Proxy = _proxySettings.GetWebProxy() };
            var token = await UploadValuesAsync<ImgurRefreshTokenResponse>(w, "https://api.imgur.com/oauth2/token.json", args);

            if (string.IsNullOrEmpty(token?.AccessToken))
                return false;

            _settings.AccessToken = token.AccessToken;
            _settings.RefreshToken = token.RefreshToken;
            _settings.ExpiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(token.ExpiresIn);
            return true;
        }

        static async Task<T> UploadValuesAsync<T>(WebClient WebClient, string Url, NameValueCollection Values)
        {
            // Task.Run done to prevent UI thread from freezing when upload fails.
            var response = await Task.Run(async () => await WebClient.UploadValuesTaskAsync(Url, Values));

            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(response));
        }

        public async Task DeleteUploadedFile(string DeleteHash)
        {
            // DeleteHash is now complete Url
            var request = WebRequest.Create(DeleteHash);

            request.Proxy = _proxySettings.GetWebProxy();
            request.Headers.Add("Authorization", await GetAuthorizationHeader());
            request.Method = "DELETE";

            var stream = (await request.GetResponseAsync()).GetResponseStream();

            if (stream != null)
            {
                var reader = new StreamReader(stream);

                var text = await reader.ReadToEndAsync();

                var res = JsonConvert.DeserializeObject<ImgurResponse>(text);

                if (res.Success)
                    return;
            }

            throw new Exception();
        }

        public string UploadServiceName { get; } = "Imgur";
    }
}