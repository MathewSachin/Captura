using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using Captura.Loc;
using Captura.Models;

namespace Captura
{
    public class FileSaveNotification : NotifyPropertyChanged, INotification
    {
        readonly FileRecentItem _recentItem;
        readonly ILocalizationProvider _loc;
        readonly IIconSet _icons;

        string _fileName;

        public FileSaveNotification(FileRecentItem RecentItem)
        {
            _recentItem = RecentItem;

            _loc = ServiceProvider.Get<ILocalizationProvider>();
            _icons = ServiceProvider.Get<IIconSet>();

            _fileName = RecentItem.FileName;

            PrimaryText = $"Saving {_recentItem.FileType} ...";
            SecondaryText = Path.GetFileName(_fileName);
        }

        public void Converting()
        {
            PrimaryText = "Converting ...";
        }

        public void Converted(string NewFileName)
        {
            SecondaryText = Path.GetFileName(NewFileName);
            _fileName = NewFileName;
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
                if (File.Exists(_fileName))
                {
                    var platformServices = ServiceProvider.Get<IPlatformServices>();

                    if (!platformServices.DeleteFile(_fileName))
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

            ServiceProvider.LaunchFile(new ProcessStartInfo(_fileName));
        }

        public void Remove() => RemoveRequested?.Invoke();

        readonly ObservableCollection<NotificationAction> _notificationActions = new ObservableCollection<NotificationAction>();

        public IEnumerable<NotificationAction> Actions => _notificationActions;

        int _progress;

        public int Progress
        {
            get => _progress;
            set => Set(ref _progress, value);
        }

        string _primaryText;

        public string PrimaryText
        {
            get => _primaryText;
            private set => Set(ref _primaryText, value);
        }

        string _secondaryText;

        public string SecondaryText
        {
            get => _secondaryText;
            private set => Set(ref _secondaryText, value);
        }

        bool _finished;

        public bool Finished
        {
            get => _finished;
            private set => Set(ref _finished, value);
        }
    }
}