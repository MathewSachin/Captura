﻿namespace Captura.Models
{
    public class CustomOverlay : TextOverlay
    {
        readonly CustomOverlaySettings _overlaySettings;

        public CustomOverlay(CustomOverlaySettings OverlaySettings) : base(OverlaySettings)
        {
            _overlaySettings = OverlaySettings;
        }

        protected override string GetText()
        {
            // TODO: Replacement Tokens
            return _overlaySettings.Text;
        }
    }
}