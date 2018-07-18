using System;
using System.Windows.Forms;

namespace Captura.Models
{
    class KeyRecord : IKeyRecord
    {
        readonly Keymap _keymap = new Keymap();

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

                case Keys.LWin:
                case Keys.RWin:
                    return _keymap.Windows;
            }

            var result = "";

            if (IsSpecialDPadCharacter)
                result = _keymap.SpecialDPad[Key - Keys.D0].ToString();

            else if (IsOem)
            {
                // Retreive first to ensure Shift is handled correctly
                var oemChar = AsOemChar;

                result = Modifiers + oemChar;
            }

            else if (IsNum)
            {
                result = Modifiers;

                // Shift has been handled already for special D-pad chars
                if (Control || Alt)
                {
                    // Ctrl, Alt shortcuts in English
                    result += AsNum;
                }
                else result += _keymap.Numbers[AsNum]; // Language specific
            }

            else if (IsAlpha)
            {
                if (Control || Alt)
                    result = Modifiers;

                if (Control || Alt)
                {
                    // Ctrl, Alt shortcuts in English
                    result += Key.ToString().ToUpper();
                }
                else
                {
                    // Language specific
                    var upperCase = Control || Alt || (Shift ^ Console.CapsLock);

                    var index = Key - Keys.A;

                    result += upperCase
                        ? _keymap.Uppercase[index]
                        : _keymap.Lowercase[index];
                }
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