using System;

namespace Captura.Models
{
    class RepeatKeyRecord : IKeyRecord
    {
        public RepeatKeyRecord(KeyRecord Repeated)
        {
            this.Repeated = Repeated;

            Increment();
        }

        public DateTime TimeStamp { get; private set; }

        public KeyRecord Repeated { get; }

        public int Repeat { get; private set; } = 1;

        public void Increment()
        {
            ++Repeat;

            TimeStamp = DateTime.Now;
        }

        public string Display => $"{Repeated} x {Repeat}";
    }
}