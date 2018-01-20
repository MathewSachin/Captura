﻿using Captura.ViewModels;
using Screna;
using System.Drawing;
using System.Drawing.Imaging;

namespace Captura.Models
{
    public class ClipboardWriter : IImageWriterItem
    {
        public void Save(Bitmap Image, ImageFormat Format, string FileName, TextLocalizer Status, RecentViewModel Recents)
        {
            using (Image)
                Image.WriteToClipboard(Format.Equals(ImageFormat.Png));

            Status.LocalizationKey = nameof(LanguageManager.ImgSavedClipboard);
        }

        public string LocalizationKey { get; } = nameof(LanguageManager.Clipboard);

        public override string ToString() => LanguageManager.Clipboard;
    }
}
