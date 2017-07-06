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
            using (var ms = new MemoryStream())
            {
                Image.Save(ms, Format);

                using (var w = new WebClient())
                {
                    w.Headers.Add("Authorization", $"Client-ID {ApiKeys.ImgurClientId}");

                    var values = new NameValueCollection
                    {
                        { "image", Convert.ToBase64String(ms.ToArray()) }
                    };

                    var response = await w.UploadValuesTaskAsync("https://api.imgur.com/3/upload.xml", values);

                    var xdoc = XDocument.Load(new MemoryStream(response));

                    var success = int.Parse(xdoc.Root.Attribute("success").Value) == 1;
                    
                    if (success)
                    {
                        var link = xdoc.Root.Element("link").Value;

                        if (Settings.Instance.CopyOutPathToClipboard)
                            link.WriteToClipboard();

                        Recents.Add(link, RecentItemType.Link, false);
                        
                        ServiceProvider.SystemTray.ShowTextNotification($"{Resources.ImgurSuccess}: {link}", Settings.Instance.ScreenShotNotifyTimeout, () => Process.Start(link));

                        Status.LocalizationKey = nameof(Resources.ImgurSuccess);
                    }
                    else
                    {
                        ServiceProvider.SystemTray.ShowTextNotification(Resources.ImgurFailed, Settings.Instance.ScreenShotNotifyTimeout, null);

                        Status.LocalizationKey = nameof(Resources.ImgurFailed);
                    }
                }
            }
        }

        public override string ToString() => Resources.Imgur;
    }
}
