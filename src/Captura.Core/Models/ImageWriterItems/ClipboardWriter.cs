using Captura.Properties;
using Captura.ViewModels;
using Screna;
using System.Drawing;
using System.Drawing.Imaging;

namespace Captura.Models
{
    public class ClipboardWriter : IImageWriterItem
    {
        ClipboardWriter() { }

        public static ClipboardWriter Instance { get; } = new ClipboardWriter();

        public void Save(Bitmap Image, ImageFormat Format, string FileName, TextLocalizer Status, RecentViewModel Recents)
        {
            using (Image)
                Image.WriteToClipboard(Format.Equals(ImageFormat.Png));

            Status.LocalizationKey = nameof(Resources.ImgSavedClipboard);
        }

        public override string ToString() => Resources.Clipboard;
    }
}
