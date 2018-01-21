using Captura.ViewModels;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

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

        public void Save(Bitmap Image, ImageFormat Format, string FileName, TextLocalizer Status, RecentViewModel Recents)
        {
            try
            {

                _settings.EnsureOutPath();

                var extension = Format.Equals(ImageFormat.Icon) ? "ico"
                    : Format.Equals(ImageFormat.Jpeg) ? "jpg"
                    : Format.ToString().ToLower();

                var fileName = FileName ?? Path.Combine(_settings.OutPath, $"{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.{extension}");

                using (Image)
                    Image.Save(fileName, Format);

                Status.LocalizationKey = nameof(LanguageManager.ImgSavedDisk);
                Recents.Add(fileName, RecentItemType.Image, false);

                if (_settings.CopyOutPathToClipboard)
                    fileName.WriteToClipboard();

                _systemTray.ShowScreenShotNotification(fileName);
            }
            catch (Exception E)
            {
                _messageProvider.ShowError($"{nameof(LanguageManager.NotSaved)}\n\n{E}");

                Status.LocalizationKey = nameof(LanguageManager.NotSaved);
            }
        }

        public string Display => LanguageManager.Instance.Disk;

        public override string ToString() => Display;
    }
}
