using System;

namespace Captura.Models
{
    public class ElapsedOverlay : TextOverlay
    {
        readonly Func<TimeSpan> _elapsed;

        public ElapsedOverlay(TextOverlaySettings OverlaySettings, Func<TimeSpan> Elapsed) : base(OverlaySettings)
        {
            _elapsed = Elapsed;
        }

        protected override string GetText() => _elapsed().ToString();
    }
}