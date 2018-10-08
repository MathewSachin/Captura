using System.Windows;
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

        void SetFile(object Sender, MouseButtonEventArgs E)
        {
            if (Sender is FrameworkElement element && element.DataContext is SoundsViewModelItem vm)
            {
                vm.SetCommand.ExecuteIfCan();
            }
        }
    }
}
