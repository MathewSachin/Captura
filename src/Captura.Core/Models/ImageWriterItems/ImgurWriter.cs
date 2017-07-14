using Captura.Properties;
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
    public class ImgurWriter : IImageWriterItem
    {
        ImgurWriter() { }

        public static ImgurWriter Instance { get; } = new ImgurWriter();

        public async void Save(Bitmap Image, ImageFormat Format, string FileName, TextLocalizer Status, RecentViewModel Recents)
        {
            var ritem = Recents.Add($"{Resources.ImgurUploading} (0%)", RecentItemType.Link, true);
                                
            using (var w = new WebClient { Proxy = Settings.Instance.GetWebProxy() })
            {
                w.UploadProgressChanged += (s, e) =>
                {
                    ritem.Display = $"{Resources.ImgurUploading} ({e.ProgressPercentage}%)";
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

                XDocument xdoc = null;

                try
                {
                    var response = await w.UploadValuesTaskAsync("https://api.imgur.com/3/upload.xml", values);

                    xdoc = XDocument.Load(new MemoryStream(response));

                    if (int.Parse(xdoc.Root.Attribute("success").Value) != 1)
                        throw new Exception("Response indicates Failure");

                    Image.Dispose();
                }
                catch (Exception E)
                {
                    ritem.Display = Resources.ImgurFailed;
                    Status.LocalizationKey = nameof(Resources.ImgurFailed);

                    ServiceProvider.MessageProvider.ShowError($"{Resources.ImgurFailed}\nWill Try to Save to Disk\n\n{E}");

                    DiskWriter.Instance.Save(Image, Format, FileName, Status, Recents);

                    return;
                }

                var link = xdoc.Root.Element("link").Value;

                if (Settings.Instance.CopyOutPathToClipboard)
                    link.WriteToClipboard();

                ritem.FilePath = ritem.Display = link;
                ritem.Saved();

                ServiceProvider.SystemTray.ShowTextNotification($"{Resources.ImgurSuccess}: {link}", Settings.Instance.ScreenShotNotifyTimeout, () => Process.Start(link));

                Status.LocalizationKey = nameof(Resources.ImgurSuccess);
            }
        }

        public override string ToString() => Resources.Imgur;
    }
}
