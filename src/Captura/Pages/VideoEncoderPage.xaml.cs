using Captura.ViewModels;

namespace Captura
{
    public partial class VideoEncoderPage
    {
        public VideoEncoderPage()
        {
            InitializeComponent();

            if (DataContext is MainViewModel vm)
            {
                vm.Refreshed += () => VideoWriterComboBox.Shake();
            }
        }
    }
}
