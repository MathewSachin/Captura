using System;

namespace Captura.MouseKeyHook
{
    class RepeatKeyRecord : IKeyRecord
    {
        readonly KeystrokesSettings _settings;

        public RepeatKeyRecord(KeyRecord Repeated, KeystrokesSettings Settings)
        {
            this.Repeated = Repeated;
            _settings = Settings;

            Increment();
        }

        public bool Control => Repeated.Control;
        public bool Shift => Repeated.Shift;
        public bool Alt => Repeated.Alt;

        public DateTime TimeStamp { get; private set; }

        public KeyRecord Repeated { get; }

        public int Repeat { get; private set; } = 1;

        public void Increment()
        {
            ++Repeat;

            TimeStamp = DateTime.Now;
        }

        public string Display => _settings.ShowRepeatCounter
            ? $"{Repeated} x {Repeat}"
            : Repeated.ToString();
    }
}