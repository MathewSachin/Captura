using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Input;
using Captura.Native;
using Screna;

namespace Captura.Models
{
    public class FileRecentItem : NotifyPropertyChanged, IRecentItem
    {
        public string FileName { get; }
        public RecentFileType FileType { get; }

        public FileRecentItem(string FileName, RecentFileType FileType, bool IsSaving = false)
        {
            this.FileName = FileName;
            this.FileType = FileType;
            this.IsSaving = IsSaving;

            Display = Path.GetFileName(FileName);

            ClickCommand = new DelegateCommand(() => ServiceProvider.LaunchFile(new ProcessStartInfo(FileName)));

            RemoveCommand = new DelegateCommand(() => RemoveRequested?.Invoke());

            var icons = ServiceProvider.Get<IIconSet>();
            var loc = ServiceProvider.Get<LanguageManager>();
            var windowService = ServiceProvider.Get<IMainWindow>();

            Icon = GetIcon(FileType, icons);
            IconColor = GetColor(FileType);

            var list = new List<RecentAction>
            {
                new RecentAction(loc.CopyPath, icons.Clipboard, () => this.FileName.WriteToClipboard())
            };

            void AddTrimMedia()
            {
                list.Add(new RecentAction(loc.Trim, icons.Trim, () => windowService.TrimMedia(FileName)));
            }

            switch (FileType)
            {
                case RecentFileType.Image:
                    list.Add(new RecentAction(loc.CopyToClipboard, icons.Clipboard, OnCopyToClipboardExecute));
                    list.Add(new RecentAction(loc.UploadToImgur, icons.Upload, OnUploadToImgurExecute));
                    list.Add(new RecentAction(loc.Edit, icons.Pencil, () => windowService.EditImage(FileName)));
                    list.Add(new RecentAction(loc.Crop, icons.Crop, () => windowService.CropImage(FileName)));
                    break;

                case RecentFileType.Audio:
                    AddTrimMedia();
                    break;

                case RecentFileType.Video:
                    AddTrimMedia();
                    list.Add(new RecentAction("Upload to YouTube", icons.YouTube, () => windowService.UploadToYouTube(FileName)));
                    break;
            }

            list.Add(new RecentAction(loc.Delete, icons.Delete, OnDelete));

            Actions = list;
        }

        async void OnUploadToImgurExecute()
        {
            if (!File.Exists(FileName))
            {
                ServiceProvider.MessageProvider.ShowError("File not Found");

                return;
            }

            var img = (Bitmap)Image.FromFile(FileName);

            await img.UploadImage();
        }

        void OnCopyToClipboardExecute()
        {
            if (!File.Exists(FileName))
            {
                ServiceProvider.MessageProvider.ShowError("File not Found");

                return;
            }

            try
            {
                var clipboard = ServiceProvider.Get<IClipboardService>();

                var img = (Bitmap)Image.FromFile(FileName);

                clipboard.SetImage(img);
            }
            catch (Exception e)
            {
                ServiceProvider.MessageProvider.ShowException(e, "Copy to Clipboard failed");
            }
        }

        void OnDelete()
        {
            if (File.Exists(FileName))
            {
                if (Shell32.FileOperation(FileName, FileOperationType.Delete, 0) != 0)
                    return;
            }

            // Remove from List
            RemoveRequested?.Invoke();
        }

        static string GetIcon(RecentFileType ItemType, IIconSet Icons)
        {
            switch (ItemType)
            {
                case RecentFileType.Audio:
                    return Icons.Music;

                case RecentFileType.Image:
                    return Icons.Image;

                case RecentFileType.Video:
                    return Icons.Video;
            }

            return null;
        }

        static string GetColor(RecentFileType ItemType)
        {
            switch (ItemType)
            {
                case RecentFileType.Audio:
                    return "DodgerBlue";

                case RecentFileType.Image:
                    return "YellowGreen";

                case RecentFileType.Video:
                    return "OrangeRed";
            }

            return null;
        }

        public string Display { get; }

        public string Icon { get; }
        public string IconColor { get; }

        bool _saving;

        public bool IsSaving
        {
            get => _saving;
            private set
            {
                _saving = value;

                OnPropertyChanged();
            }
        }

        public void Saved()
        {
            IsSaving = false;
        }

        public event Action RemoveRequested;

        public ICommand ClickCommand { get; }
        public ICommand RemoveCommand { get; }

        public IEnumerable<RecentAction> Actions { get; }
    }
}