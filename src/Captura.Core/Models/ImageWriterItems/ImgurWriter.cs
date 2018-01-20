using Captura.ViewModels;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Xml.Linq;

namespace Captura.Models
{
    public class ImgurWriter : NotifyPropertyChanged, IImageWriterItem
    {
        readonly DiskWriter _diskWriter;
        readonly ISystemTray _systemTray;
        readonly IMessageProvider _messageProvider;

        public ImgurWriter(DiskWriter DiskWriter, ISystemTray SystemTray, IMessageProvider MessageProvider)
        {
            _diskWriter = DiskWriter;
            _systemTray = SystemTray;
            _messageProvider = MessageProvider;

            TranslationSource.Instance.PropertyChanged += (s, e) => OnPropertyChanged(nameof(Display));
        }

        public async void Save(Bitmap Image, ImageFormat Format, string FileName, TextLocalizer Status, RecentViewModel Recents)
        {
            var ritem = Recents.Add($"{LanguageManager.ImgurUploading} (0%)", RecentItemType.Link, true);
                                
            using (var w = new WebClient { Proxy = Settings.Instance.GetWebProxy() })
            {
                w.UploadProgressChanged += (s, e) =>
                {
                    ritem.Display = $"{LanguageManager.ImgurUploading} ({e.ProgressPercentage}%)";
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

                    Image.Dispose();
                }
                catch (Exception E)
                {
                    ritem.Display = LanguageManager.ImgurFailed;
                    Status.LocalizationKey = nameof(LanguageManager.ImgurFailed);

                    var yes = _messageProvider.ShowYesNo($"{LanguageManager.ImgurFailed}\n{E.Message}\n\nDo you want to Save to Disk?", "Imgur Upload Failed");

                    if (yes)
                        _diskWriter.Save(Image, Format, FileName, Status, Recents);

                    return;
                }

                var link = xdoc.Root.Element("link").Value;

                if (Settings.Instance.CopyOutPathToClipboard)
                    link.WriteToClipboard();

                ritem.FilePath = ritem.Display = link;
                ritem.Saved();

                _systemTray.ShowTextNotification($"{LanguageManager.ImgurSuccess}: {link}", Settings.Instance.ScreenShotNotifyTimeout, () => Process.Start(link));

                Status.LocalizationKey = nameof(LanguageManager.ImgurSuccess);
            }
        }

        public string Display => LanguageManager.Imgur;

        public override string ToString() => Display;
    }
}
