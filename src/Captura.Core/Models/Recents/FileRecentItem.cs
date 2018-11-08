using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using Screna;

namespace Captura.Models
{
    public class FileRecentItem : NotifyPropertyChanged, IRecentItem
    {
        public string FileName { get; }
        public RecentItemType FileType { get; }

        public FileRecentItem(string FileName, RecentItemType FileType, bool IsSaving = false)
        {
            this.FileName = FileName;
            this.FileType = FileType;
            this.IsSaving = IsSaving;

            Display = Path.GetFileName(FileName);

            ClickCommand = new DelegateCommand(() => ServiceProvider.LaunchFile(new ProcessStartInfo(FileName)));

            RemoveCommand = new DelegateCommand(() => RemoveRequested?.Invoke());

            var icons = ServiceProvider.Get<IIconSet>();
            var loc = ServiceProvider.Get<LanguageManager>();

            Icon = GetIcon(FileType, icons);
            IconColor = GetColor(FileType);

            Actions = new[]
            {
                new RecentAction(loc.CopyPath, icons.Clipboard, () => this.FileName.WriteToClipboard()),
                new RecentAction(loc.Delete, icons.Delete, OnDelete),
            };
        }

        void OnDelete()
        {
            if (!ServiceProvider.MessageProvider.ShowYesNo($"Are you sure you want to Delete: {FileName}?", "Confirm Deletion"))
                return;

            try
            {
                File.Delete(FileName);
            }
            catch (Exception e)
            {
                ServiceProvider.MessageProvider.ShowException(e, $"Could not Delete: {FileName}");

                return;
            }

            // Remove from List
            RemoveRequested?.Invoke();
        }

        static string GetIcon(RecentItemType ItemType, IIconSet Icons)
        {
            switch (ItemType)
            {
                case RecentItemType.Audio:
                    return Icons.Music;

                case RecentItemType.Image:
                    return Icons.Image;

                case RecentItemType.Video:
                    return Icons.Video;
            }

            return null;
        }

        static string GetColor(RecentItemType ItemType)
        {
            switch (ItemType)
            {
                case RecentItemType.Audio:
                    return "DodgerBlue";

                case RecentItemType.Image:
                    return "YellowGreen";

                case RecentItemType.Video:
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