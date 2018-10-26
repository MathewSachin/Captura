using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;

namespace Captura
{
    public class NotificationViewModel : NotifyPropertyChanged, INotification
    {
        public event Action Click;
        public event Action RemoveRequested;

        public void Remove() => RemoveRequested?.Invoke();

        public void RaiseClick() => Click?.Invoke();

        readonly ObservableCollection<NotificationAction> _actions = new ObservableCollection<NotificationAction>();

        public IReadOnlyCollection<NotificationAction> Actions => _actions;

        readonly SynchronizationContext _syncContext = SynchronizationContext.Current;

        public NotificationAction AddAction()
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
            set
            {
                _primaryText = value;
                
                OnPropertyChanged();
            }
        }

        public string SecondaryText
        {
            get => _secondaryText;
            set
            {
                _secondaryText = value;
                
                OnPropertyChanged();
            }
        }

        bool _finished, _success;

        public bool Finished
        {
            get => _finished;
            set
            {
                _finished = value;
                
                OnPropertyChanged();
            }
        }

        public bool Success
        {
            get => _success;
            set
            {
                _success = value;
                
                OnPropertyChanged();
            }
        }
    }
}