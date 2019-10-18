using System;
using System.Diagnostics;

namespace Captura.Models
{
    public class FpsOverlay : TextOverlay
    {
        readonly Stopwatch _sw = new Stopwatch();
        int _frames;
        readonly TimeSpan _diff = TimeSpan.FromSeconds(1);
        int _fps;

        public FpsOverlay(TextOverlaySettings OverlaySettings) : base(OverlaySettings)
        {
        }

        protected override string GetText()
        {
            if (!_sw.IsRunning)
            {
                _sw.Start();

                return "-";
            }

            ++_frames;

            if (_sw.Elapsed > _diff)
            {
                _fps = (int)Math.Round(_frames / _sw.Elapsed.TotalSeconds);
                _frames = 0;

                _sw.Restart();
            }

            return _fps.ToString();
        }
    }
}