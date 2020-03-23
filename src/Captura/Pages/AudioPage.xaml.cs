using Captura.Models;

namespace Captura
{
    public partial class AudioPage
    {
        public AudioPage()
        {
            IsVisibleChanged += (S, E) =>
            {
                var audioSourceVm = ServiceProvider.Get<AudioSourceViewModel>();

                audioSourceVm.ListeningPeakLevel = IsVisible;
            };

            InitializeComponent();
        }
    }
}
