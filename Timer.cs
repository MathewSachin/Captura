using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Captura
{
    public partial class Timer : Label
    {
        DispatcherTimer DTimer;
        int Seconds = 0, Minutes = 0;

        public Timer()
        {
            DTimer = new DispatcherTimer();
            DTimer.Interval = TimeSpan.FromSeconds(1);
            DTimer.Tick += new EventHandler(Tick);

            Content = "00:00";

            HorizontalContentAlignment = HorizontalAlignment.Center;
            VerticalContentAlignment = VerticalAlignment.Center;
        }

        void Tick(object sender, EventArgs e)
        {
            Seconds++;

            if (Seconds == 60)
            {
                Seconds = 0;
                Minutes++;
            }

            Content = string.Format("{0:D2}:{1:D2}", Minutes, Seconds);
        }

        public void Start() { DTimer.Start(); }

        public void Stop() { DTimer.Stop(); }

        public void Reset()
        {
            DTimer.Stop();

            Seconds = Minutes = 0;

            Content = "00:00";
        }
    }
}
