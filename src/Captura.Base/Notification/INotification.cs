using System;
using System.Collections.Generic;

namespace Captura
{
    public interface INotification
    {
        int Progress { get; }

        string PrimaryText { get; }

        string SecondaryText { get; }

        bool Finished { get; }

        IEnumerable<NotificationAction> Actions { get; }

        void RaiseClick();

        void Remove();

        event Action RemoveRequested;
    }
}