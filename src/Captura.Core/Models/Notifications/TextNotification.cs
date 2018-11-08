using System;
using System.Collections.Generic;

namespace Captura
{
    public class TextNotification : INotification
    {
        readonly Action _onClick;

        public TextNotification(string PrimaryText, Action OnClick = null, string SecondaryText = null)
        {
            _onClick = OnClick;

            this.PrimaryText = PrimaryText;
            this.SecondaryText = SecondaryText;
        }

        public int Progress => 0;

        public string PrimaryText { get; }
        public string SecondaryText { get; }

        bool INotification.Finished => true;

        public IEnumerable<NotificationAction> Actions { get; } = new NotificationAction[0];

        public void RaiseClick() => _onClick?.Invoke();

        public void Remove() => RemoveRequested?.Invoke();

        public event Action RemoveRequested;
    }
}