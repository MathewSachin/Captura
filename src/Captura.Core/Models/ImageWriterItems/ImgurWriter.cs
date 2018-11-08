using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Screna;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ImgurWriter : NotifyPropertyChanged, IImageWriterItem
    {
        readonly DiskWriter _diskWriter;
        readonly ISystemTray _systemTray;
        readonly IMessageProvider _messageProvider;
        readonly Settings _settings;
        readonly LanguageManager _loc;
        readonly IRecentList _recentList;
        readonly IIconSet _icons;

        public ImgurWriter(DiskWriter DiskWriter,
            ISystemTray SystemTray,
            IMessageProvider MessageProvider,
            Settings Settings,
            LanguageManager LanguageManager,
            IRecentList RecentList,
            IIconSet Icons)
        {
            _diskWriter = DiskWriter;
            _systemTray = SystemTray;
            _messageProvider = MessageProvider;
            _settings = Settings;
            _loc = LanguageManager;
            _recentList = RecentList;
            _icons = Icons;

            LanguageManager.LanguageChanged += L => RaisePropertyChanged(nameof(Display));
        }

        public async Task Save(Bitmap Image, ImageFormat Format, string FileName)
        {
            var response = await Save(Image, Format);

            switch (response)
            {
                case ImgurUploadResponse uploadResponse:
                    var recentItem = _recentList.Add(uploadResponse.Data.Link, RecentItemType.Link, false);
                    recentItem.DeleteHash = uploadResponse.Data.DeleteHash;

                    // Copy path to clipboard only when clipboard writer is off
                    if (_settings.CopyOutPathToClipboard && !ServiceProvider.Get<ClipboardWriter>().Active)
                        uploadResponse.Data.Link.WriteToClipboard();
                    break;

                case Exception e:
                    if (!_diskWriter.Active)
                    {
                        ServiceProvider.Get<IMainWindow>().IsVisible = true;

                        var yes = _messageProvider.ShowYesNo(
                            $"{_loc.ImgurFailed}\n{e.Message}\n\nDo you want to Save to Disk?", "Imgur Upload Failed");

                        if (yes)
                            await _diskWriter.Save(Image, Format, FileName);
                    }
                    break;
            }
        }

        public async Task DeleteUploadedFile(string DeleteHash)
        {
            var request = WebRequest.Create($"https://api.imgur.com/3/image/{DeleteHash}");

            request.Proxy = _settings.Proxy.GetWebProxy();
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

        async Task<string> GetAuthorizationHeader()
        {
            if (_settings.Imgur.Anonymous)
            {
                return $"Client-ID {ApiKeys.ImgurClientId}";
            }

            if (string.IsNullOrWhiteSpace(_settings.Imgur.AccessToken))
            {
                throw new Exception("Not logged in to Imgur");
            }

            if (_settings.Imgur.IsExpired())
            {
                if (!await RefreshToken())
                {
                    throw new Exception("Failed to Refresh Imgur token");
                }
            }

            return $"Bearer {_settings.Imgur.AccessToken}";
        }

        // Returns ImgurUploadResponse on success, Exception on failure
        public async Task<object> Save(Bitmap Image, ImageFormat Format)
        {
            var progressItem = _systemTray.ShowNotification(true);
            progressItem.PrimaryText = _loc.ImgurUploading;
            
            using (var w = new WebClient { Proxy = _settings.Proxy.GetWebProxy() })
            {
                w.UploadProgressChanged += (S, E) =>
                {
                    progressItem.Progress = E.ProgressPercentage;
                };

                try
                {
                    w.Headers.Add("Authorization", await GetAuthorizationHeader());
                }
                catch (Exception e)
                {
                    return e;
                }

                NameValueCollection values;

                using (var ms = new MemoryStream())
                {
                    Image.Save(ms, Format);

                    values = new NameValueCollection
                    {
                        { "image", Convert.ToBase64String(ms.ToArray()) }
                    };
                }

                ImgurUploadResponse uploadResponse;

                try
                {
                    uploadResponse = await UploadValuesAsync<ImgurUploadResponse>(w, "https://api.imgur.com/3/upload.json", values);
                    
                    if (!uploadResponse.Success)
                    {
                        throw new Exception("Response indicates Failure");
                    }
                }
                catch (Exception e)
                {
                    progressItem.Finished = true;
                    progressItem.Success = false;

                    progressItem.PrimaryText = _loc.ImgurFailed;

                    return e;
                }

                var link = uploadResponse.Data.Link;

                progressItem.Finished = true;
                progressItem.Success = true;
                progressItem.PrimaryText = _loc.ImgurSuccess;
                progressItem.SecondaryText = link;

                var copyLinkAction = progressItem.AddAction();
                copyLinkAction.Name = _loc.CopyToClipboard;
                copyLinkAction.Icon = _icons.Link;
                copyLinkAction.Click += () => link.WriteToClipboard();

                progressItem.Click += () => Process.Start(link);

                return uploadResponse;
            }
        }

        static async Task<T> UploadValuesAsync<T>(WebClient WebClient, string Url, NameValueCollection Values)
        {
            // Task.Run done to prevent UI thread from freezing when upload fails.
            var response = await Task.Run(async () => await WebClient.UploadValuesTaskAsync(Url, Values));

            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(response));
        }

        public void Authorize()
        {
            // response_type should be token, other types have been deprecated.
            // access_token and refresh_token will be available in query string parameters attached to redirect URL entered during registration.
            // eg: http://example.com#access_token=ACCESS_TOKEN&token_type=Bearer&expires_in=3600
            // currently unable to retrieve them.
            Process.Start($"https://api.imgur.com/oauth2/authorize?response_type=token&client_id={ApiKeys.ImgurClientId}");
        }
        
        public async Task<bool> RefreshToken()
        {
            var args = new NameValueCollection
            {
                { "refresh_token", _settings.Imgur.RefreshToken },
                { "client_id", ApiKeys.ImgurClientId },
                { "client_secret", ApiKeys.ImgurSecret },
                { "grant_type", "refresh_token" }
            };

            using (var w = new WebClient { Proxy = _settings.Proxy.GetWebProxy() })
            {
                var token = await UploadValuesAsync<ImgurRefreshTokenResponse>(w, "https://api.imgur.com/oauth2/token.json", args);

                if (string.IsNullOrEmpty(token?.AccessToken))
                    return false;

                _settings.Imgur.AccessToken = token.AccessToken;
                _settings.Imgur.RefreshToken = token.RefreshToken;
                _settings.Imgur.ExpiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(token.ExpiresIn);
                return true;
            }
        }

        public string Display => "Imgur";

        bool _active;

        public bool Active
        {
            get => _active;
            set
            {
                _active = value;

                OnPropertyChanged();
            }
        }

        public override string ToString() => Display;
    }
}
