using Captura.ViewModels;
using Screna;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace Captura.Models
{
    public class ClipboardWriter : NotifyPropertyChanged, IImageWriterItem
    {
        public Task Save(Bitmap Image, ImageFormat Format, string FileName, TextLocalizer Status, RecentViewModel Recents)
        {
            Image.WriteToClipboard(Format.Equals(ImageFormat.Png));

            Status.LocalizationKey = nameof(LanguageManager.ImgSavedClipboard);

            LanguageManager.Instance.LanguageChanged += L => RaisePropertyChanged(nameof(Display));

            return Task.CompletedTask;
        }

        public string Display => LanguageManager.Instance.Clipboard;

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
