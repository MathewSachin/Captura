using Captura.Models;

namespace Captura.ViewModels
{
    public class CustomImageOverlaysViewModel : ArraySettingsViewModel<CustomImageOverlaySettings>
    {
        public CustomImageOverlaysViewModel() : base("CustomImageOverlays.json")
        {
        }
    }
}