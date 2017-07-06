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

                var ritem = Recents.Add(Resources.ImgurUploading, RecentItemType.Link, true);
                                
                using (var w = new WebClient())
                {
                    w.Headers.Add("Authorization", $"Client-ID {ApiKeys.ImgurClientId}");

                    var values = new NameValueCollection
                    {
                        { "image", Convert.ToBase64String(ms.ToArray()) }
                    };

                    var success = false;
                    XDocument xdoc = null;

                    try
                    {
                        var response = await w.UploadValuesTaskAsync("https://api.imgur.com/3/upload.xml", values);

                        xdoc = XDocument.Load(new MemoryStream(response));

                        success = int.Parse(xdoc.Root.Attribute("success").Value) == 1;
                    }
                    catch
                    {
                        success = false;
                    }

                    if (success)
                    {
                        var link = xdoc.Root.Element("link").Value;

                        if (Settings.Instance.CopyOutPathToClipboard)
                            link.WriteToClipboard();

                        ritem.FilePath = ritem.Display = link;
                        ritem.Saved();

                        ServiceProvider.SystemTray.ShowTextNotification($"{Resources.ImgurSuccess}: {link}", Settings.Instance.ScreenShotNotifyTimeout, () => Process.Start(link));

                        Status.LocalizationKey = nameof(Resources.ImgurSuccess);
                    }
                    else
                    {
                        ServiceProvider.SystemTray.ShowTextNotification(Resources.ImgurFailed, Settings.Instance.ScreenShotNotifyTimeout, null);

                        ritem.Display = Resources.ImgurFailed;
                        Status.LocalizationKey = nameof(Resources.ImgurFailed);
                    }
                }
            }
        }

        public override string ToString() => Resources.Imgur;
    }
}
