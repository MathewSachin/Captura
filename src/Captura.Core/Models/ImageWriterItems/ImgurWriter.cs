using Captura.ViewModels;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;

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
                w.UploadProgressChanged += (s, e) =>
                {
                    progressItem.Progress = e.ProgressPercentage;
                };

                w.Headers.Add("Authorization", $"Client-ID {ApiKeys.ImgurClientId}");

                NameValueCollection values;

                using (var ms = new MemoryStream())
                {
                    Image.Save(ms, Format);

                    values = new NameValueCollection
                    {
                        { "image", Convert.ToBase64String(ms.ToArray()) }
                    };
                }

                XDocument xdoc;

                try
                {
                    var response = await w.UploadValuesTaskAsync("https://api.imgur.com/3/upload.xml", values);

                    xdoc = XDocument.Load(new MemoryStream(response));

                    var xAttribute = xdoc.Root?.Attribute("success");

                    if (xAttribute == null || int.Parse(xAttribute.Value) != 1)
                        throw new Exception("Response indicates Failure");
                }
                catch (Exception e)
                {
                    progressItem.Finished = true;
                    progressItem.Success = false;

                    progressItem.PrimaryText = _loc.ImgurFailed;

                    return e;
                }

                var link = xdoc.Root.Element("link").Value;

                progressItem.Finished = true;
                progressItem.Success = true;
                progressItem.PrimaryText = _loc.ImgurSuccess;
                progressItem.SecondaryText = link;

                progressItem.RegisterClick(() => Process.Start(link));

                return link;
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
