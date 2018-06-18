using Captura.Models;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Screna;

namespace Captura.ViewModels
{
    public class RecentItemViewModel : NotifyPropertyChanged
    {
        public RecentItemViewModel(string FilePath, RecentItemType ItemType, bool IsSaving)
        {
            this.FilePath = FilePath;

            Display = ItemType == RecentItemType.Link ? FilePath : Path.GetFileName(FilePath);

            this.IsSaving = IsSaving;
            this.ItemType = ItemType;
            IsImage = ItemType == RecentItemType.Image;
            IsTrimmable = ItemType == RecentItemType.Audio || ItemType == RecentItemType.Video;

            InitCommands();
        }

        void InitCommands()
        {
            RemoveCommand = new DelegateCommand(() => OnRemove?.Invoke(), !IsSaving);

            OpenCommand = new DelegateCommand(() =>
            {
                ServiceProvider.LaunchFile(new ProcessStartInfo(FilePath));
            }, !IsSaving);

            CopyPathCommand = new DelegateCommand(() =>
            {
                FilePath.WriteToClipboard();
            }, !IsSaving);

            DeleteCommand = new DelegateCommand(async () =>
            {
                if (!ServiceProvider.MessageProvider.ShowYesNo($"Are you sure you want to Delete: {FilePath}?", "Confirm Deletion"))
                    return;

                try
                {
                    if (ItemType == RecentItemType.Link && !string.IsNullOrWhiteSpace(DeleteHash))
                    {
                        await ServiceProvider.Get<ImgurWriter>().DeleteUploadedFile(DeleteHash);
                    }
                    else File.Delete(FilePath);
                }
                catch (Exception e)
                {
                    ServiceProvider.MessageProvider.ShowError(e.ToString(), $"Could not Delete: {FilePath}");

                    return;
                }

                // Remove from List
                OnRemove?.Invoke();

            }, !IsSaving);

            CopyToClipboardCommand = new DelegateCommand(() =>
            {
                if (!File.Exists(FilePath))
                {
                    ServiceProvider.MessageProvider.ShowError("File not Found");

                    return;
                }

                try
                {
                    var img = (Bitmap) Image.FromFile(FilePath);

                    img.WriteToClipboard();
                }
                catch (Exception e)
                {
                    ServiceProvider.MessageProvider.ShowError(e.ToString(), "Copy to Clipboard failed");
                }
            });

            UploadToImgurCommand = new DelegateCommand(async () =>
            {
                if (!File.Exists(FilePath))
                {
                    ServiceProvider.MessageProvider.ShowError("File not Found");

                    return;
                }

                try
                {
                    var img = (Bitmap)Image.FromFile(FilePath);

                    var imgur = ServiceProvider.Get<ImgurWriter>();

                    var response = await imgur.Save(img, ImageFormat.Png);

                    switch (response)
                    {
                        case Exception ex:
                            ServiceProvider.MessageProvider.ShowError(ex.ToString(), "Upload to Imgur failed");
                            break;

                        case ImgurUploadResponse uploadResponse:
                            uploadResponse.Data.Link.WriteToClipboard();
                            break;
                    }
                }
                catch (Exception e)
                {
                    ServiceProvider.MessageProvider.ShowError(e.ToString(), "Upload to Imgur failed");
                }
            });
        }

        string _filePath;

        public string FilePath
        {
            get => _filePath;
            set
            {
                _filePath = value;

                OnPropertyChanged();
            }
        }

        string _display;

        public string Display
        {
            get => _display;
            set
            {
                _display = value;

                OnPropertyChanged();
            }
        }

        public RecentItemType ItemType { get; }

        public bool IsImage { get; }

        public bool IsTrimmable { get; }

        public string DeleteHash { get; set; }

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
            if (IsSaving == false)
                return;

            IsSaving = false;

            RemoveCommand.RaiseCanExecuteChanged(true);
            OpenCommand.RaiseCanExecuteChanged(true);
            DeleteCommand.RaiseCanExecuteChanged(true);
            CopyPathCommand.RaiseCanExecuteChanged(true);
        }

        public DelegateCommand RemoveCommand { get; private set; }

        public DelegateCommand OpenCommand { get; private set; }

        public DelegateCommand CopyPathCommand { get; private set; }

        public DelegateCommand CopyToClipboardCommand { get; private set; }

        public DelegateCommand UploadToImgurCommand { get; private set; }

        public DelegateCommand DeleteCommand { get; private set; }

        public event Action OnRemove;
    }
}