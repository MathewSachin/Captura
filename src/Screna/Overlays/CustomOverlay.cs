namespace Captura.Video
{
    public class CustomOverlay : TextOverlay
    {
        readonly CustomOverlaySettings _overlaySettings;
        
        public CustomOverlay(CustomOverlaySettings OverlaySettings) : base(OverlaySettings)
        {
            _overlaySettings = OverlaySettings;
        }

        protected override string GetText() => _overlaySettings.Text;
    }
}