using System;
using System.Windows.Input;

namespace Captura
{
    public class NotificationAction
    {
        public NotificationAction()
        {
            ClickCommand = new DelegateCommand(() => Click?.Invoke());
        }

        public string Color { get; set; }

        public string Icon { get; set; }

        public string Name { get; set; }

        public ICommand ClickCommand { get; }

        public event Action Click;
    }
}