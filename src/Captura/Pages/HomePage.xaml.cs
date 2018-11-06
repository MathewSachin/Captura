using Captura.ViewModels;

namespace Captura
{
    public partial class HomePage
    {
        public HomePage()
        {
            InitializeComponent();

            if (DataContext is MainViewModel vm)
            {
                vm.Refreshed += () =>
                {
                    AudioDropdown.Shake();

                    VideoWriterComboBox.Shake();
                };
            }
        }
    }
}
