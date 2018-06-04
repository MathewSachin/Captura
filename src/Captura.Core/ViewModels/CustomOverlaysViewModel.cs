namespace Captura.ViewModels
{
    public class CustomOverlaysViewModel : CustomOverlaysBaseViewModel<CustomOverlaySettings>
    {
        public CustomOverlaysViewModel(Settings Settings) : base(Settings.TextOverlays)
        {
        }
    }
}