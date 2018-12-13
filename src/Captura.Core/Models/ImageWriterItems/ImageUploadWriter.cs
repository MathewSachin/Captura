using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using Screna;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ImageUploadWriter : NotifyPropertyChanged, IImageWriterItem
    {
        readonly DiskWriter _diskWriter;
        readonly ISystemTray _systemTray;
        readonly IMessageProvider _messageProvider;
        readonly Settings _settings;
        readonly LanguageManager _loc;
        readonly IRecentList _recentList;

        readonly IImageUploader _imgUploader;

        public ImageUploadWriter(DiskWriter DiskWriter,
            ISystemTray SystemTray,
            IMessageProvider MessageProvider,
            Settings Settings,
            LanguageManager LanguageManager,
            IRecentList RecentList,
            IImageUploader ImgUploader)
        {
            _imgUploader = ImgUploader;

            _diskWriter = DiskWriter;
            _systemTray = SystemTray;
            _messageProvider = MessageProvider;
            _settings = Settings;
            _loc = LanguageManager;
            _recentList = RecentList;

            LanguageManager.LanguageChanged += L => RaisePropertyChanged(nameof(Display));
        }

        public async Task Save(Bitmap Image, ImageFormat Format, string FileName)
        {
            var response = await Save(Image, Format);

            switch (response)
            {
                case UploadResult uploadResult:
                    var link = uploadResult.Url;

                    // Copy path to clipboard only when clipboard writer is off
                    if (_settings.CopyOutPathToClipboard && !ServiceProvider.Get<ClipboardWriter>().Active)
                        link.WriteToClipboard();
                    break;

                case Exception e:
                    if (!_diskWriter.Active)
                    {
                        ServiceProvider.Get<IMainWindow>().IsVisible = true;

                        var yes = _messageProvider.ShowYesNo(
                            $"{e.Message}\n\nDo you want to Save to Disk?", _loc.ImageUploadFailed);

                        if (yes)
                            await _diskWriter.Save(Image, Format, FileName);
                    }
                    break;
            }
        }

        // Returns UploadResult on success, Exception on failure
        public async Task<object> Save(Bitmap Image, ImageFormat Format)
        {
            var progressItem = new ImageUploadNotification();
            _systemTray.ShowNotification(progressItem);

            try
            {
                var uploadResult = await _imgUploader.Upload(Image, Format, M => progressItem.Progress = M);

                var link = uploadResult.Url;

                _recentList.Add(new UploadRecentItem(link, uploadResult.DeleteLink, _imgUploader));

                progressItem.RaiseFinished(link);

                return uploadResult;
            }
            catch (Exception e)
            {
                progressItem.RaiseFailed();

                return e;
            }
        }

        public string Display => "Upload";

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
