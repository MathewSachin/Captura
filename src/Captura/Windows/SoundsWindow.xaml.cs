using System.Windows.Input;
using Captura.ViewModels;

namespace Captura
{
    public partial class SoundsWindow
    {
        public SoundsWindow()
        {
            InitializeComponent();
        }

        void SetNormalFile(object Sender, MouseButtonEventArgs E)
        {
            if (DataContext is SoundsViewModel vm)
            {
                vm.SetNormalCommand.ExecuteIfCan();
            }
        }

        void SetShotFile(object Sender, MouseButtonEventArgs E)
        {
            if (DataContext is SoundsViewModel vm)
            {
                vm.SetShotCommand.ExecuteIfCan();
            }
        }

        void SetErrorFile(object Sender, MouseButtonEventArgs E)
        {
            if (DataContext is SoundsViewModel vm)
            {
                vm.SetErrorCommand.ExecuteIfCan();
            }
        }

        void SetNotificationFile(object Sender, MouseButtonEventArgs E)
        {
            if (DataContext is SoundsViewModel vm)
            {
                vm.SetNotificationCommand.ExecuteIfCan();
            }
        }
    }
}
