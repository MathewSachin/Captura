using System;

namespace Captura
{
    public interface INotification
    {
        int Progress { get; set; }

        string PrimaryText { get; set; }

        string SecondaryText { get; set; }

        bool Finished { get; set; }

        bool Success { get; set; }

        event Action Click;

        void Remove();
    }
}