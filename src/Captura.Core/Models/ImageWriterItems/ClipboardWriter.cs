using Captura.ViewModels;
using Screna;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace Captura.Models
{
    public class ClipboardWriter : NotifyPropertyChanged, IImageWriterItem
    {
        readonly ISystemTray _systemTray;

        public ClipboardWriter(ISystemTray SystemTray)
        {
            _systemTray = SystemTray;
        }

        public Task Save(Bitmap Image, ImageFormat Format, string FileName, RecentViewModel Recents)
        {
            Image.WriteToClipboard(Format.Equals(ImageFormat.Png));

            _systemTray.ShowMessage(LanguageManager.Instance.ImgSavedClipboard);

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
