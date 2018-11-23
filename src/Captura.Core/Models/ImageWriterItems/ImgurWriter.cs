using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using Screna;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ImgurWriter : NotifyPropertyChanged, IImageWriterItem
    {
        readonly DiskWriter _diskWriter;
        readonly ISystemTray _systemTray;
        readonly IMessageProvider _messageProvider;
        readonly Settings _settings;
        readonly LanguageManager _loc;
        readonly IRecentList _recentList;

        readonly IImageUploader _imgurUploader;

        public ImgurWriter(DiskWriter DiskWriter,
            ISystemTray SystemTray,
            IMessageProvider MessageProvider,
            Settings Settings,
            LanguageManager LanguageManager,
            IRecentList RecentList,
            ImgurUploader ImgurUploader)
        {
            _imgurUploader = ImgurUploader;

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

                    _recentList.Add(new ImgurRecentItem(link, uploadResult.DeleteLink));

                    // Copy path to clipboard only when clipboard writer is off
                    if (_settings.CopyOutPathToClipboard && !ServiceProvider.Get<ClipboardWriter>().Active)
                        link.WriteToClipboard();
                    break;

                case Exception e:
                    if (!_diskWriter.Active)
                    {
                        ServiceProvider.Get<IMainWindow>().IsVisible = true;

                        var yes = _messageProvider.ShowYesNo(
                            $"{_loc.ImgurFailed}\n{e.Message}\n\nDo you want to Save to Disk?", "Imgur Upload Failed");

                        if (yes)
                            await _diskWriter.Save(Image, Format, FileName);
                    }
                    break;
            }
        }

        public async Task DeleteUploadedFile(string DeleteHash)
        {
            // DeleteHash is now complete Url
            await _imgurUploader.DeleteUploadedFile(DeleteHash);
        }

        // Returns UploadResult on success, Exception on failure
        public async Task<object> Save(Bitmap Image, ImageFormat Format)
        {
            var progressItem = new ImgurNotification();
            _systemTray.ShowNotification(progressItem);

            try
            {
                var uploadResult = await _imgurUploader.Upload(Image, Format, M => progressItem.Progress = M);

                progressItem.RaiseFinished(uploadResult.Url);

                return uploadResult;
            }
            catch (Exception e)
            {
                progressItem.RaiseFailed();

                return e;
            }
        }

        public string Display => "Imgur";

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
