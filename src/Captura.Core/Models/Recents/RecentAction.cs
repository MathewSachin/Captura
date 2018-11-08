using System;
using System.Windows.Input;

namespace Captura.Models
{
    public class RecentAction
    {
        public RecentAction(string Name, string Icon, Action OnClick)
        {
            this.Name = Name;
            this.Icon = Icon;

            ClickCommand = new DelegateCommand(() => OnClick?.Invoke());
        }

        public string Name { get; set; }

        public string Icon { get; }

        public ICommand ClickCommand { get; }
    }
}