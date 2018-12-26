using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using Captura.Models;
using Captura.Native;

namespace Captura
{
    public class FileSaveNotification : NotifyPropertyChanged, INotification
    {
        readonly FileRecentItem _recentItem;
        readonly LanguageManager _loc;
        readonly IIconSet _icons;

        public FileSaveNotification(FileRecentItem RecentItem)
        {
            _recentItem = RecentItem;

            _loc = ServiceProvider.Get<LanguageManager>();
            _icons = ServiceProvider.Get<IIconSet>();

            PrimaryText = $"Saving {_recentItem.FileType} ...";
            SecondaryText = Path.GetFileName(RecentItem.FileName);
        }

        public void Saved()
        {
            var deleteAction = new NotificationAction
            {
                Icon = _icons.Delete,
                Name = _loc.Delete,
                Color = "LightPink"
            };

            deleteAction.Click += () =>
            {
                if (File.Exists(_recentItem.FileName))
                {
                    if (Shell32.FileOperation(_recentItem.FileName, FileOperationType.Delete, 0) != 0)
                        return;
                }

                Remove();

                OnDelete?.Invoke();
            };

            _notificationActions.Add(deleteAction);

            PrimaryText = _recentItem.FileType == RecentFileType.Video ? _loc.VideoSaved : _loc.AudioSaved;
            Finished = true;
        }

        public event Action RemoveRequested;

        public event Action OnDelete;

        public void RaiseClick()
        {
            if (_recentItem.IsSaving)
                return;

            ServiceProvider.LaunchFile(new ProcessStartInfo(_recentItem.FileName));
        }

        public void Remove() => RemoveRequested?.Invoke();

        readonly ObservableCollection<NotificationAction> _notificationActions = new ObservableCollection<NotificationAction>();

        public IEnumerable<NotificationAction> Actions => _notificationActions;

        int INotification.Progress => 0;

        string _primaryText;

        public string PrimaryText
        {
            get => _primaryText;
            private set
            {
                _primaryText = value;

                OnPropertyChanged();
            }
        }

        public string SecondaryText { get; }

        bool _finished;

        public bool Finished
        {
            get => _finished;
            private set
            {
                _finished = value;

                OnPropertyChanged();
            }
        }
    }
}