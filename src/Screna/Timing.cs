﻿using System;
using System.Diagnostics;

namespace Screna
{
    public class Timing
    {
        TimeSpan _addend;
        Stopwatch _stopwatch = new Stopwatch();

        public TimeSpan Elapsed => _stopwatch.Elapsed + _addend;

        public void Start()
        {
            _stopwatch.Start();
        }

        public void Pause()
        {
            _addend += _stopwatch.Elapsed;

            _stopwatch.Reset();
        }

        public void Stop()
        {
            _addend = TimeSpan.Zero;

            _stopwatch.Reset();
        }
    }
}