using Captura.ViewModels;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Screna;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DiskWriter : NotifyPropertyChanged, IImageWriterItem
    {
        readonly ISystemTray _systemTray;
        readonly IMessageProvider _messageProvider;
        readonly Settings _settings;
        readonly LanguageManager _loc;

        public DiskWriter(ISystemTray SystemTray, IMessageProvider MessageProvider, Settings Settings, LanguageManager Loc)
        {
            _systemTray = SystemTray;
            _messageProvider = MessageProvider;
            _settings = Settings;
            _loc = Loc;

            Loc.LanguageChanged += L => RaisePropertyChanged(nameof(Display));
        }

        public Task Save(Bitmap Image, ImageFormat Format, string FileName, RecentViewModel Recents)
        {
            try
            {
                _settings.EnsureOutPath();

                var extension = Format.Equals(ImageFormat.Icon) ? "ico"
                    : Format.Equals(ImageFormat.Jpeg) ? "jpg"
                    : Format.ToString().ToLower();

                var fileName = FileName ?? Path.Combine(_settings.OutPath, $"{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.{extension}");

                Image.Save(fileName, Format);
                
                Recents.Add(fileName, RecentItemType.Image, false);

                // Copy path to clipboard only when clipboard writer is off
                if (_settings.CopyOutPathToClipboard && !ServiceProvider.Get<ClipboardWriter>().Active)
                    fileName.WriteToClipboard();

                _systemTray.ShowScreenShotNotification(fileName);
            }
            catch (Exception e)
            {
                _messageProvider.ShowException(e, _loc.NotSaved);
            }

            return Task.CompletedTask;
        }

        public string Display => _loc.Disk;

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
