namespace Captura.ViewModels
{
    public class CustomOverlaysViewModel : OverlayListViewModel<CustomOverlaySettings>
    {
        public CustomOverlaysViewModel(Settings Settings) : base(Settings.TextOverlays)
        {
        }
    }
}