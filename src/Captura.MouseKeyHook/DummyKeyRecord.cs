using System;

namespace Captura.Models
{
    public class DummyKeyRecord : IKeyRecord
    {
        public DummyKeyRecord(string Display)
        {
            this.Display = Display;

            TimeStamp = DateTime.Now;
        }

        public DateTime TimeStamp { get; }

        public string Display { get; }
    }
}