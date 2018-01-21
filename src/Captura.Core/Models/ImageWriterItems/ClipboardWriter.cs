using Captura.ViewModels;
using Screna;
using System.Drawing;
using System.Drawing.Imaging;

namespace Captura.Models
{
    public class ClipboardWriter : NotifyPropertyChanged, IImageWriterItem
    {
        public void Save(Bitmap Image, ImageFormat Format, string FileName, TextLocalizer Status, RecentViewModel Recents)
        {
            using (Image)
                Image.WriteToClipboard(Format.Equals(ImageFormat.Png));

            Status.LocalizationKey = nameof(LanguageManager.ImgSavedClipboard);

            LanguageManager.Instance.LanguageChanged += L => RaisePropertyChanged(nameof(Display));
        }

        public string Display => LanguageManager.Instance.Clipboard;

        public override string ToString() => Display;
    }
}
