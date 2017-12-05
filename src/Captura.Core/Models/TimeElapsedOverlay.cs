using System;

namespace Captura.Models
{
    public class TimeElapsedOverlay : TextOverlay
    {
        readonly Func<TimeSpan> _elapsed;

        public TimeElapsedOverlay(Func<TimeSpan> Elapsed) : base(Settings.Instance.TimeElapsedOverlay)
        {
            _elapsed = Elapsed;
        }

        protected override string GetText() => _elapsed().ToString();
    }
}
