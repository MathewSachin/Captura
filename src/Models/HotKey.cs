using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Captura
{
    class Hotkey
    {
        public Action Work { get; }

        public Hotkey(Modifiers Modifiers, Keys Key, Action Work)
        {
            this.Key = Key;
            this.Modifiers = Modifiers;
            this.Work = Work;

            Register();
        }
        
        public bool IsRegistered { get; private set; }

        public ushort ID { get; private set; }

        public void Register()
        {
            var uid = Guid.NewGuid().ToString("N");
            ID = GlobalAddAtom(uid);

            if (IsRegistered)
                return;
            
            if (RegisterHotKey(IntPtr.Zero, ID, Modifiers, Key))
            {
                IsRegistered = true;
            }
            else
            {
                GlobalDeleteAtom(ID);
                ID = 0;
            }
        }

        public Keys Key { get; set; }

        public Modifiers Modifiers { get; set; }

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
        const string User32 = "user32", Kernel32 = "kernel32.dll";

        [DllImport(Kernel32)]
        static extern ushort GlobalAddAtom(string Text);

        [DllImport(Kernel32)]
        static extern ushort GlobalDeleteAtom(ushort Atom);

        [DllImport(User32)]
        static extern bool UnregisterHotKey(IntPtr Hwnd, int Id);

        [DllImport(User32)]
        static extern bool RegisterHotKey(IntPtr Hwnd, int Id, Modifiers Modifiers, Keys VirtualKey);
        #endregion
    }
}