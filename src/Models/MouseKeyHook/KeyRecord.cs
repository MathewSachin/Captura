using System;
using System.Windows.Forms;

namespace Screna
{
    class KeyRecord
    {
        public KeyRecord(KeyEventArgs KeyEventArgs)
        {
            TimeStamp = DateTime.Now;

            Key = KeyEventArgs.KeyCode;
            Control = KeyEventArgs.Control;
            Shift = KeyEventArgs.Shift;
            Alt = KeyEventArgs.Alt;
        }

        public DateTime TimeStamp { get; }

        public Keys Key { get; }

        public bool Control { get; }
        public bool Shift { get; }
        public bool Alt { get; }
        
        public bool IsAlpha => Key >= Keys.A && Key <= Keys.Z;

        public bool IsNum => (Key >= Keys.D0 && Key <= Keys.D9) || (Key >= Keys.NumPad0 && Key <= Keys.NumPad9);

        public bool IsSpecialDPadCharacter => !Alt && !Control && Shift && IsNum;

        public string AsSpecialDPadCharacter => new[]
        {
            ')',
            '!',
            '@',
            '#',
            '$',
            '%',
            '^',
            '&',
            '*',
            '('
        }[Key - Keys.D0].ToString();

        public int AsNum
        {
            get
            {
                if (Key >= Keys.D0 && Key <= Keys.D9)
                    return Key - Keys.D0;

                return Key - Keys.NumPad0;
            }
        }
        
        public override string ToString()
        {
            var modifiers = $"{(Control ? "Ctrl + " : "")}{(Shift ? "Shift + " : "")}{(Alt ? "Alt + " : "")}";

            var result = "";

            if (IsSpecialDPadCharacter)
                result = AsSpecialDPadCharacter;

            else if (IsNum)
                result = modifiers + AsNum;

            else if (IsAlpha)
            {
                var upperCase = Control || Alt || (Shift ^ Console.CapsLock);

                var keyString = Key.ToString();

                if (Control || Alt)
                    result = modifiers;

                result += upperCase ? keyString.ToUpper() : keyString.ToLower();
            }

            else
            {
                result = modifiers;

                switch (Key)
                {
                    case Keys.Decimal:
                    case Keys.OemPeriod:
                        result += ".";
                        break;

                    case Keys.Oemcomma:
                        result += ",";
                        break;

                    case Keys.Add:
                        result += "+";
                        break;

                    case Keys.OemMinus:
                    case Keys.Subtract:
                        result += "-";
                        break;

                    case Keys.Multiply:
                        result += "*";
                        break;

                    case Keys.Divide:
                        result += "/";
                        break;

                    case Keys.NumLock:
                        result += $"NumLock: {(Console.NumberLock ? "On" : "Off")}";
                        break;

                    case Keys.CapsLock:
                        result += $"CapsLock: {(Console.CapsLock ? "On" : "Off")}";
                        break;

                    default:
                        result += Key;
                        break;
                }
            }

            return result;
        }
    }
}