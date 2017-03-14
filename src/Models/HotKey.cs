using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Captura
{
    public class Hotkey
    {
        public Action Work { get; }

        public Hotkey(string Description, Modifiers Modifiers, Keys Key, Action Work)
        {
            this.Description = Description;
            this.Key = Key;
            this.Modifiers = Modifiers;
            this.Work = Work;

            Register();
        }
        
        public bool IsRegistered { get; private set; }

        public ushort ID { get; private set; }

        public void Register()
        {
            if (IsRegistered || Key == Keys.None)
                return;

            // Generate Unique ID
            var uid = Guid.NewGuid().ToString("N");
            ID = GlobalAddAtom(uid);
            
            if (RegisterHotKey(IntPtr.Zero, ID, Modifiers, Key))
                IsRegistered = true;
            else
            {
                GlobalDeleteAtom(ID);
                ID = 0;
            }
        }

        public string Description { get; }

        public Keys Key { get; private set; }

        public Modifiers Modifiers { get; private set; }

        public void Change(Keys Key, Modifiers Modifiers)
        {
            Unregister();

            this.Key = Key;
            this.Modifiers = Modifiers;

            Register();
        }

        public void Unregister()
        {
            if (!IsRegistered)
                return;

            if (UnregisterHotKey(IntPtr.Zero, ID))
            {
                IsRegistered = false;

                GlobalDeleteAtom(ID);
                ID = 0;
            }
        }

        #region Native
        const string User32 = "user32", Kernel32 = "kernel32";

        [DllImport(Kernel32)]
        static extern ushort GlobalAddAtom(string Text);

        [DllImport(Kernel32)]
        static extern ushort GlobalDeleteAtom(ushort Atom);

        [DllImport(User32)]
        static extern bool UnregisterHotKey(IntPtr Hwnd, int Id);

        [DllImport(User32)]
        static extern bool RegisterHotKey(IntPtr Hwnd, int Id, Modifiers Modifiers, Keys VirtualKey);
        #endregion

        public override string ToString()
        {
            var text = "";

            if (Modifiers.HasFlag(Modifiers.Ctrl))
                text += "Ctrl + ";

            if (Modifiers.HasFlag(Modifiers.Alt))
                text += "Alt + ";

            if (Modifiers.HasFlag(Modifiers.Shift))
                text += "Shift + ";

            // Handle Number keys
            if (Key >= Keys.D0 && Key <= Keys.D9)
                text += (Key - Keys.D0);
            else if (Key >= Keys.NumPad0 && Key <= Keys.NumPad9)
                text += (Key - Keys.NumPad0);
            else text += Key;

            return text;
        }
    }
}