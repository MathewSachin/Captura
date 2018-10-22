using Captura.ViewModels;

namespace Captura
{
    public partial class AudioPage
    {
        public AudioPage()
        {
            InitializeComponent();

            ServiceProvider.Get<MainViewModel>().Refreshed += () =>
            {
                AudioSourcesPanel.Shake();
            };
        }
    }
}
