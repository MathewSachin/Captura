using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Captura.Loc;
using Captura.Models;

namespace Captura
{
    public class ImageUploadNotification : NotifyPropertyChanged, INotification
    {
        readonly ILocalizationProvider _loc;

        public ImageUploadNotification()
        {
            _loc = ServiceProvider.Get<ILocalizationProvider>();

            PrimaryText = _loc.ImageUploading;
        }

        public event Action RemoveRequested;

        public void Remove() => RemoveRequested?.Invoke();

        public void RaiseClick()
        {
            if (_link != null)
            {
                Process.Start(_link);
            }
        }

        public void RaiseFailed()
        {
            Finished = true;

            PrimaryText = _loc.ImageUploadFailed;
        }

        string _link;

        public void RaiseFinished(string Link)
        {
            _link = Link;

            Finished = true;
            PrimaryText = _loc.ImageUploadSuccess;
            SecondaryText = Link;

            var icons = ServiceProvider.Get<IIconSet>();

            var copyLinkAction = AddAction();
            copyLinkAction.Name = _loc.CopyToClipboard;
            copyLinkAction.Icon = icons.Link;
            copyLinkAction.Click += Link.WriteToClipboard;
        }

        readonly ObservableCollection<NotificationAction> _actions = new ObservableCollection<NotificationAction>();

        public IEnumerable<NotificationAction> Actions => _actions;

        readonly SyncContextManager _syncContext = new SyncContextManager();

        NotificationAction AddAction()
        {
            var action = new NotificationAction();

            _syncContext.Run(() => _actions.Add(action));

            return action;
        }

        int _progress;

        public int Progress
        {
            get => _progress;
            set => Set(ref _progress, value);
        }

        string _primaryText, _secondaryText;

        public string PrimaryText
        {
            get => _primaryText;
            private set => Set(ref _primaryText, value);
        }

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