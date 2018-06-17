using Captura.ViewModels;
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

namespace Captura.Models
{
    public class ImgurWriter : NotifyPropertyChanged, IImageWriterItem
    {
        readonly DiskWriter _diskWriter;
        readonly ISystemTray _systemTray;
        readonly IMessageProvider _messageProvider;
        readonly Settings _settings;
        readonly LanguageManager _loc;

        public ImgurWriter(DiskWriter DiskWriter,
            ISystemTray SystemTray,
            IMessageProvider MessageProvider,
            Settings Settings,
            LanguageManager LanguageManager)
        {
            _diskWriter = DiskWriter;
            _systemTray = SystemTray;
            _messageProvider = MessageProvider;
            _settings = Settings;
            _loc = LanguageManager;

            LanguageManager.Instance.LanguageChanged += L => RaisePropertyChanged(nameof(Display));
        }

        public async Task Save(Bitmap Image, ImageFormat Format, string FileName, RecentViewModel Recents)
        {
            var response = await Save(Image, Format);

            switch (response)
            {
                case string link:
                    Recents.Add(link, RecentItemType.Link, false);

                    // Copy path to clipboard only when clipboard writer is off
                    if (_settings.CopyOutPathToClipboard && !ServiceProvider.Get<ClipboardWriter>().Active)
                        link.WriteToClipboard();
                    break;

                case Exception e:
                    if (!_diskWriter.Active)
                    {
                        ServiceProvider.Get<IMainWindow>().IsVisible = true;

                        var yes = _messageProvider.ShowYesNo(
                            $"{_loc.ImgurFailed}\n{e.Message}\n\nDo you want to Save to Disk?", "Imgur Upload Failed");

                        if (yes)
                            await _diskWriter.Save(Image, Format, FileName, Recents);
                    }
                    break;
            }
        }

        // Returns Link on success, Exception on failure
        public async Task<object> Save(Bitmap Image, ImageFormat Format)
        {
            var progressItem = _systemTray.ShowProgress();
            progressItem.PrimaryText = _loc.ImgurUploading;

            using (var w = new WebClient { Proxy = _settings.Proxy.GetWebProxy() })
            {
                w.UploadProgressChanged += (S, E) =>
                {
                    progressItem.Progress = E.ProgressPercentage;
                };

                if (_settings.Imgur.Anonymous)
                {
                    w.Headers.Add("Authorization", $"Client-ID {ApiKeys.ImgurClientId}");
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(_settings.Imgur.AccessToken))
                    {
                        return new Exception("Not logged in to Imgur");
                    }

                    if (_settings.Imgur.IsExpired())
                    {
                        if (!await RefreshToken())
                        {
                            return new Exception("Failed to Refresh Imgur token");
                        }
                    }

                    w.Headers.Add("Authorization", $"Bearer {_settings.Imgur.AccessToken}");
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

                progressItem.RegisterClick(() => Process.Start(link));

                return link;
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

        public string Display => _loc.Imgur;

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
