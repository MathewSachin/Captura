using System;

namespace Captura.Video
{
    public class ElapsedOverlay : TextOverlay
    {
        readonly Func<TimeSpan> _elapsed;

        public ElapsedOverlay(TextOverlaySettings OverlaySettings, Func<TimeSpan> Elapsed) : base(OverlaySettings)
        {
            _elapsed = Elapsed;
        }

        protected override string GetText() //=> _elapsed().ToString();
        {
            DateTime dateTime = DateTime.Now;
            string currentTime = String.Format("{0:00}'{1:00}.{2:000}''",
            dateTime.Minute, dateTime.Second, dateTime.Millisecond);
            return currentTime;
        }
    }
}