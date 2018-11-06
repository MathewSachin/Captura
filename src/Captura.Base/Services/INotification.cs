using System;
using System.Collections.Generic;

namespace Captura
{
    public interface INotification
    {
        int Progress { get; set; }

        string PrimaryText { get; set; }

        string SecondaryText { get; set; }

        bool Finished { get; set; }

        bool Success { get; set; }

        IReadOnlyCollection<NotificationAction> Actions { get; }

        NotificationAction AddAction();

        event Action Click;

        void Remove();
    }
}