using System;

namespace Captura
{
    public class NotificationAction
    {
        public string Icon { get; set; }

        public string Name { get; set; }

        public event Action Click;
    }
}