using System;
using System.Diagnostics;

namespace Captura.Models
{
    public class FpsManager : NotifyPropertyChanged, IFpsManager
    {
        readonly Stopwatch _sw = new Stopwatch();
        readonly TimeSpan _diff = TimeSpan.FromSeconds(1);
        long _frames;
        int _fps;

        public void OnFrame()
        {
            if (!_sw.IsRunning)
            {
                _sw.Start();
                Fps = 0;

                return;
            }

            ++_frames;

            if (_sw.Elapsed < _diff) 
                return;

            Fps = (int)Math.Round(_frames / _sw.Elapsed.TotalSeconds);
            _frames = 0;

            _sw.Restart();
        }

        public int Fps
        {
            get => _fps;
            private set => Set(ref _fps, value);
        }
    }
}