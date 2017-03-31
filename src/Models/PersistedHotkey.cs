using System;
using System.Windows.Forms;

namespace Captura
{
    public class PersistedHotkey : Hotkey
    {
        string KeySetting, ModSetting;

        public PersistedHotkey(string Description, string ModSetting, string KeySetting, Action Work)
            : base((Modifiers)Settings.Instance[ModSetting], (Keys)Settings.Instance[KeySetting], Work)
        {
            this.Description = Description;
            this.KeySetting = KeySetting;
            this.ModSetting = ModSetting;
        }

        public string Description { get; }

        public override void Change(Keys Key, Modifiers Modifiers)
        {
            base.Change(Key, Modifiers);

            Settings.Instance[KeySetting] = Key;
            Settings.Instance[ModSetting] = Modifiers;
        }
    }
}