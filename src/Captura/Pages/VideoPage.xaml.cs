using Captura.ViewModels;

namespace Captura
{
    public partial class VideoPage
    {
        public VideoPage()
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
