using System;
using System.Windows.Forms;

namespace Captura.Models
{
    public class KeyRecord : IKeyRecord
    {
        public KeyRecord(KeyEventArgs KeyEventArgs)
        {
            TimeStamp = DateTime.Now;

            Key = KeyEventArgs.KeyCode;
            Control = KeyEventArgs.Control;
            Shift = KeyEventArgs.Shift;
            Alt = KeyEventArgs.Alt;

            Display = GetDisplay();
        }

        public DateTime TimeStamp { get; }

        public Keys Key { get; }
        
        public bool Control { get; }
        public bool Shift { get; private set; }
        public bool Alt { get; }
        
        public string Display { get; }

        bool IsAlpha => Key >= Keys.A && Key <= Keys.Z;

        bool IsNum => (Key >= Keys.D0 && Key <= Keys.D9) || (Key >= Keys.NumPad0 && Key <= Keys.NumPad9);

        bool IsSpecialDPadCharacter => !Alt && !Control && Shift && (Key >= Keys.D0 && Key <= Keys.D9);

        bool IsOem => Key.ToString().Contains("Oem");

        static readonly char[] DPadChars = {
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
        };

        char AsSpecialDPadCharacter => DPadChars[Key - Keys.D0];

        int AsNum
        {
            get
            {
                if (Key >= Keys.D0 && Key <= Keys.D9)
                    return Key - Keys.D0;

                return Key - Keys.NumPad0;
            }
        }
        
        string Oem(char ShiftChar, char NonShiftChar)
        {
            if (Shift)
            {
                Shift = false;

                return ShiftChar.ToString();
            }

            return NonShiftChar.ToString();
        }

        string AsOemChar
        {
            get
            {
                switch (Key)
                {
                    case Keys.Oemtilde:
                        return Oem('~', '`');

                    case Keys.OemMinus:
                        return Oem('_', '-');

                    case Keys.Oemplus:
                        return Oem('+', '=');

                    case Keys.Oem6:
                        return Oem('}', ']');

                    case Keys.OemOpenBrackets:
                        return Oem('{', '[');

                    case Keys.Oem5:
                        return Oem('|', '\\');

                    case Keys.Oem7:
                        return Oem('\'', '"');

                    case Keys.Oem1:
                        return Oem(':', ';');

                    case Keys.OemPeriod:
                        return Oem('>', '.');

                    case Keys.Oemcomma:
                        return Oem('<', ',');

                    case Keys.OemQuestion:
                        return Oem('?', '/');
                }

                return Key.ToString();
            }
        }
        
        string Modifiers => $"{(Control ? "Ctrl + " : "")}{(Shift ? "Shift + " : "")}{(Alt ? "Alt + " : "")}";

        string GetDisplay()
        {
            switch (Key)
            {
                case Keys.Shift:
                case Keys.ShiftKey:
                case Keys.LShiftKey:
                case Keys.RShiftKey:
                    return "Shift";

                case Keys.Control:
                case Keys.ControlKey:
                case Keys.LControlKey:
                case Keys.RControlKey:
                    return "Ctrl";

                case Keys.Alt:
                case Keys.Menu:
                case Keys.LMenu:
                case Keys.RMenu:
                    return "Alt";

                case Keys.LWin:
                case Keys.RWin:
                    return "Win";
            }

            var result = "";

            if (IsSpecialDPadCharacter)
                result = AsSpecialDPadCharacter.ToString();

            else if (IsOem)
                result = Modifiers + AsOemChar;

            else if (IsNum)
                result = Modifiers + AsNum;

            else if (IsAlpha)
            {
                var upperCase = Control || Alt || (Shift ^ Console.CapsLock);

                var keyString = Key.ToString();

                if (Control || Alt)
                    result = Modifiers;

                result += upperCase ? keyString.ToUpper() : keyString.ToLower();
            }

            else
            {
                result = Modifiers;

                switch (Key)
                {
                    case Keys.Decimal:
                        result += ".";
                        break;

                    case Keys.Add:
                        result += "+";
                        break;

                    case Keys.Subtract:
                        result += "-";
                        break;

                    case Keys.Multiply:
                        result += "*";
                        break;

                    case Keys.Divide:
                        result += "/";
                        break;

                    case Keys.Escape:
                        result += "Esc";
                        break;

                    case Keys.Delete:
                        result += "Del";
                        break;

                    case Keys.PageUp:
                        result += "Pg Up";
                        break;

                    case Keys.PageDown:
                        result += "Pg Dn";
                        break;

                    case Keys.PrintScreen:
                        result += "Prt Sc";
                        break;

                    case Keys.VolumeMute:
                        result += "Mute";
                        break;

                    case Keys.VolumeUp:
                        result += "Vol +";
                        break;
                    
                    case Keys.VolumeDown:
                        result += "Vol -";
                        break;

                    case Keys.Up:
                        result += " ↑ ";
                        break;

                    case Keys.Down:
                        result += " ↓ ";
                        break;

                    case Keys.Left:
                        result += " ← ";
                        break;

                    case Keys.Right:
                        result += " → ";
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

        public override string ToString() => Display;
    }
}