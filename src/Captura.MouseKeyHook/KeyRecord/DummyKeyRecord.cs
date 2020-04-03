using System;

namespace Captura.MouseKeyHook
{
    class DummyKeyRecord : IKeyRecord
    {
        public DummyKeyRecord(string Display)
        {
            this.Display = Display;

            TimeStamp = DateTime.Now;
        }

        public bool Control => false;
        public bool Shift => false;
        public bool Alt => false;

        public DateTime TimeStamp { get; }

        public string Display { get; }
    }
}