using System;
using System.Drawing;
using System.Drawing.Imaging;
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
        readonly IRecentList _recentList;

        public DiskWriter(ISystemTray SystemTray,
            IMessageProvider MessageProvider,
            Settings Settings,
            LanguageManager Loc,
            IRecentList RecentList)
        {
            _systemTray = SystemTray;
            _messageProvider = MessageProvider;
            _settings = Settings;
            _loc = Loc;
            _recentList = RecentList;

            Loc.LanguageChanged += L => RaisePropertyChanged(nameof(Display));
        }

        public Task Save(Bitmap Image, ImageFormat Format, string FileName)
        {
            try
            {
                _settings.EnsureOutPath();

                var extension = Format.Equals(ImageFormat.Icon) ? "ico"
                    : Format.Equals(ImageFormat.Jpeg) ? "jpg"
                    : Format.ToString().ToLower();

                var fileName = _settings.GetFileName(extension, FileName);

                Image.Save(fileName, Format);
                
                _recentList.Add(fileName, RecentItemType.Image, false);

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
