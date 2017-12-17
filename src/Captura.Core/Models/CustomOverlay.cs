using System;

namespace Captura.Models
{
    public class CustomOverlay : TextOverlay
    {
        readonly CustomOverlaySettings _overlaySettings;

        readonly Func<TimeSpan> _elapsed;

        public CustomOverlay(CustomOverlaySettings OverlaySettings, Func<TimeSpan> Elapsed) : base(OverlaySettings)
        {
            _overlaySettings = OverlaySettings;
            _elapsed = Elapsed;
        }

        protected override string GetText()
        {
            var text = _overlaySettings.Text;

            text = text?.Replace("%elapsed%", _elapsed().ToString());

            return text;
        }
    }
}