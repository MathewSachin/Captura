using System.Threading.Tasks;
using Captura.Loc;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ClipboardWriter : NotifyPropertyChanged, IImageWriterItem
    {
        readonly ISystemTray _systemTray;
        readonly IClipboardService _clipboard;
        readonly ILocalizationProvider _loc;

        public ClipboardWriter(ISystemTray SystemTray,
            ILocalizationProvider Loc,
            IClipboardService Clipboard)
        {
            _systemTray = SystemTray;
            _loc = Loc;
            _clipboard = Clipboard;

            Loc.LanguageChanged += L => RaisePropertyChanged(nameof(Display));
        }

        public Task Save(IBitmapImage Image, ImageFormats Format, string FileName)
        {
            _clipboard.SetImage(Image);

            _systemTray.ShowNotification(new TextNotification(_loc.ImgSavedClipboard));

            return Task.CompletedTask;
        }

        public string Display => _loc.Clipboard;

        bool _active;

        public bool Active
        {
            get => _active;
            set => Set(ref _active, value);
        }

        public override string ToString() => Display;
    }
}
