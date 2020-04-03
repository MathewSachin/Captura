using System;
using System.Windows.Forms;

namespace Captura.MouseKeyHook
{
    class KeyRecord : IKeyRecord
    {
        readonly KeyEventArgs _eventArgs;
        readonly KeymapViewModel _keymap;

        readonly bool _capsLock;

        public KeyRecord(KeyEventArgs KeyEventArgs, KeymapViewModel Keymap)
        {
            _keymap = Keymap;
            _eventArgs = KeyEventArgs;
            TimeStamp = DateTime.Now;

            Key = KeyEventArgs.KeyCode;
            Control = KeyEventArgs.Control;
            Shift = KeyEventArgs.Shift;
            Alt = KeyEventArgs.Alt;

            _capsLock = Console.CapsLock;

            Display = GetDisplay();
        }

        public DateTime TimeStamp { get; }

        public Keys Key { get; }

        bool IsNum => (Key >= Keys.D0 && Key <= Keys.D9) || (Key >= Keys.NumPad0 && Key <= Keys.NumPad9);

        int AsNum
        {
            get
            {
                if (Key >= Keys.D0 && Key <= Keys.D9)
                    return Key - Keys.D0;

                return Key - Keys.NumPad0;
            }
        }

        public bool Control { get; }
        public bool Shift { get; }
        public bool Alt { get; }
        
        public string Display { get; }
        
        string Modifiers
        {
            get
            {
                var result = "";

                if (Control)
                {
                    result += $"{_keymap.Control} + ";
                }

                if (Shift)
                {
                    result += $"{_keymap.Shift} + ";
                }

                if (Alt)
                {
                    result += $"{_keymap.Alt} + ";
                }

                return result;
            }
        }

        string GetDisplay()
        {
            switch (Key)
            {
                case Keys.Shift:
                case Keys.ShiftKey:
                case Keys.LShiftKey:
                case Keys.RShiftKey:
                    return _keymap.Shift;

                case Keys.Control:
                case Keys.ControlKey:
                case Keys.LControlKey:
                case Keys.RControlKey:
                    return _keymap.Control;

                case Keys.Alt:
                case Keys.Menu:
                case Keys.LMenu:
                case Keys.RMenu:
                    return _keymap.Alt;
            }

            var found = _keymap.Find(Key, new ModifierStates
            {
                CapsLock = _capsLock,
                Alt = Alt,
                Control = Control,
                Shift = Shift
            });

            if (found != null)
                return found;

            if (_eventArgs.Modifiers == 0)
            {
                found = _keymap.Find(Key, new ModifierStates
                {
                    CapsLock = _capsLock
                });

                return found ?? Key.ToString();
            }

            if (IsNum)
            {
                return Modifiers + AsNum;
            }

            // Alphabet
            if (Key >= Keys.A && Key <= Keys.Z)
            {
                return Modifiers + Key.ToString().ToUpper();
            }

            found = _keymap.Find(Key, new ModifierStates
            {
                CapsLock = _capsLock
            });

            return Modifiers + (found ?? Key.ToString());
        }

        public override string ToString() => Display;
    }
}