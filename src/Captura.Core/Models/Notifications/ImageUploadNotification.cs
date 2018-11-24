using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using Screna;

namespace Captura
{
    public class ImageUploadNotification : NotifyPropertyChanged, INotification
    {
        readonly LanguageManager _loc;

        public ImageUploadNotification()
        {
            _loc = ServiceProvider.Get<LanguageManager>();

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

        readonly SynchronizationContext _syncContext = SynchronizationContext.Current;

        NotificationAction AddAction()
        {
            var action = new NotificationAction();

            if (_syncContext != null)
            {
                _syncContext.Post(S => _actions.Add(action), null);
            }
            else _actions.Add(action);

            return action;
        }

        int _progress;

        public int Progress
        {
            get => _progress;
            set
            {
                _progress = value;

                OnPropertyChanged();
            }
        }

        string _primaryText, _secondaryText;

        public string PrimaryText
        {
            get => _primaryText;
            private set
            {
                _primaryText = value;

                OnPropertyChanged();
            }
        }

        public string SecondaryText
        {
            get => _secondaryText;
            private set
            {
                _secondaryText = value;

                OnPropertyChanged();
            }
        }

        bool _finished;

        public bool Finished
        {
            get => _finished;
            set
            {
                _finished = value;

                OnPropertyChanged();
            }
        }
    }
}