using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Captura.MouseKeyHook
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ModifierStates
    {
        public static ModifierStates GetCurrent()
        {
            var modifiers = Keyboard.Modifiers;

            var modifierStates = new ModifierStates
            {
                Control = modifiers.HasFlag(ModifierKeys.Control),
                Shift = modifiers.HasFlag(ModifierKeys.Shift),
                Alt = modifiers.HasFlag(ModifierKeys.Alt),
                CapsLock = Console.CapsLock
            };

            return modifierStates;
        }

        public string ToString(KeymapViewModel Keymap)
        {
            var pressed = new List<string>();

            if (Control)
                pressed.Add(Keymap.Control);

            if (Shift)
                pressed.Add(Keymap.Shift);

            if (Alt)
                pressed.Add(Keymap.Alt);

            if (pressed.Count == 0)
                return "";

            return string.Join(" + ", pressed);
        }

        public static ModifierStates Empty { get; } = new ModifierStates();

        public bool Control { get; set; }

        public bool Shift { get; set; }

        public bool Alt { get; set; }

        public bool CapsLock { get; set; }
    }
}