using Captura.Video;

namespace Captura.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class CensorOverlaysViewModel : OverlayListViewModel<CensorOverlaySettings>
    {
        public CensorOverlaysViewModel(Settings Settings) : base(Settings.Censored)
        {
        }
    }
}