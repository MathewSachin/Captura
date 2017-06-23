using Captura.Properties;
using Captura.ViewModels;
using Screna;
using System;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Xml;
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
                    var clientId = "";

                    w.Headers.Add("Authorization", $"Client-ID {clientId}");

                    var values = new NameValueCollection
                    {
                        { "image", Convert.ToBase64String(ms.ToArray()) }
                    };

                    var response = await w.UploadValuesTaskAsync("https://api.imgur.com/3/upload.xml", values);

                    var xdoc = XDocument.Load(new MemoryStream(response));

                    var success = int.Parse(xdoc.Root.Attribute("success").Value) == 1;
                    
                    if (success)
                    {
                        Status.LocalizationKey = nameof(Resources.ScreenShotSaved);
                    }
                    else
                    {
                        Status.LocalizationKey = nameof(Resources.NotSaved);
                    }
                }
            }
        }

        public override string ToString() => Resources.Clipboard;
    }
}
