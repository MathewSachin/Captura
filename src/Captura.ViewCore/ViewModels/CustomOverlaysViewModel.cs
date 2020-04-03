using Captura.Video;

namespace Captura.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class CustomOverlaysViewModel : OverlayListViewModel<CustomOverlaySettings>
    {
        public CustomOverlaysViewModel(Settings Settings) : base(Settings.TextOverlays)
        {
        }
    }
}