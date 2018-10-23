using System.Windows.Input;
using Captura.ViewModels;

namespace Captura
{
    public partial class OutputFolderControl
    {
        public OutputFolderControl()
        {
            InitializeComponent();
        }

        void SelectTargetFolder(object Sender, MouseButtonEventArgs E)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.SelectOutputFolderCommand.ExecuteIfCan();
            }
        }
    }
}
