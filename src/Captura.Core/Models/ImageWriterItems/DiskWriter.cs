using Captura.ViewModels;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Captura.Models
{
    public class DiskWriter : IImageWriterItem
    {
        readonly ISystemTray _systemTray;
        readonly IMessageProvider _messageProvider;

        public DiskWriter(ISystemTray SystemTray, IMessageProvider MessageProvider)
        {
            _systemTray = SystemTray;
            _messageProvider = MessageProvider;
        }

        public void Save(Bitmap Image, ImageFormat Format, string FileName, TextLocalizer Status, RecentViewModel Recents)
        {
            try
            {
                Settings.Instance.EnsureOutPath();

                var extension = Format.Equals(ImageFormat.Icon) ? "ico"
                    : Format.Equals(ImageFormat.Jpeg) ? "jpg"
                    : Format.ToString().ToLower();

                var fileName = FileName ?? Path.Combine(Settings.Instance.OutPath, $"{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.{extension}");

                using (Image)
                    Image.Save(fileName, Format);

                Status.LocalizationKey = nameof(LanguageManager.ImgSavedDisk);
                Recents.Add(fileName, RecentItemType.Image, false);

                if (Settings.Instance.CopyOutPathToClipboard)
                    fileName.WriteToClipboard();

                _systemTray.ShowScreenShotNotification(fileName);
            }
            catch (Exception E)
            {
                _messageProvider.ShowError($"{nameof(LanguageManager.NotSaved)}\n\n{E}");

                Status.LocalizationKey = nameof(LanguageManager.NotSaved);
            }
        }

        public override string ToString() => LanguageManager.Disk;
    }
}
