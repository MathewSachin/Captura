using System;
using System.Collections.Generic;

namespace Captura.Models
{
    class FakeNotification : INotification
    {
        public int Progress { get; set; }

        public string PrimaryText
        {
            get => null;
            set => Console.WriteLine(value);
        }

        public string SecondaryText
        {
            get => null;
            set => Console.WriteLine(value);
        }

        public bool Finished { get; set; }
        public bool Success { get; set; }

        public IReadOnlyCollection<NotificationAction> Actions { get; } = new NotificationAction[0];

        public NotificationAction AddAction()
        {
            return new NotificationAction();
        }

#pragma warning disable CS0067
        public event Action Click;
#pragma warning restore CS0067

        public void Remove() { }
    }
}