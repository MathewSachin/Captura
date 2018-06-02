using Captura.ViewModels;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace Captura.Models
{
    public class DiskWriter : NotifyPropertyChanged, IImageWriterItem
    {
        readonly ISystemTray _systemTray;
        readonly IMessageProvider _messageProvider;
        readonly Settings _settings;

        public DiskWriter(ISystemTray SystemTray, IMessageProvider MessageProvider, Settings Settings)
        {
            _systemTray = SystemTray;
            _messageProvider = MessageProvider;
            _settings = Settings;

            LanguageManager.Instance.LanguageChanged += L => RaisePropertyChanged(nameof(Display));
        }

        public Task Save(Bitmap Image, ImageFormat Format, string FileName, TextLocalizer Status, RecentViewModel Recents)
        {
            try
            {

                _settings.EnsureOutPath();

                var extension = Format.Equals(ImageFormat.Icon) ? "ico"
                    : Format.Equals(ImageFormat.Jpeg) ? "jpg"
                    : Format.ToString().ToLower();

                var fileName = FileName ?? Path.Combine(_settings.OutPath, $"{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.{extension}");

                Image.Save(fileName, Format);

                Status.LocalizationKey = nameof(LanguageManager.ImgSavedDisk);
                Recents.Add(fileName, RecentItemType.Image, false);

                // Copy path to clipboard only when clipboard writer is off
                if (_settings.CopyOutPathToClipboard && !ServiceProvider.Get<ClipboardWriter>().Active)
                    fileName.WriteToClipboard();

                _systemTray.ShowScreenShotNotification(fileName);
            }
            catch (Exception e)
            {
                _messageProvider.ShowError($"{nameof(LanguageManager.NotSaved)}\n\n{e}");

                Status.LocalizationKey = nameof(LanguageManager.NotSaved);
            }

            return Task.CompletedTask;
        }

        public string Display => LanguageManager.Instance.Disk;

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
