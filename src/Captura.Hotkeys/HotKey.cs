using System;
using System.Linq;
using System.Windows.Forms;
using Captura.Native;

namespace Captura.Hotkeys
{
    public class Hotkey : NotifyPropertyChanged
    {
        Service _service;

        public Service Service
        {
            get => _service;
            set => Set(ref _service, value);
        }

        public Hotkey(HotkeyModel Model)
        {
            Service = HotKeyManager.AllServices.FirstOrDefault(M => M.ServiceName == Model.ServiceName);
            Key = Model.Key;
            Modifiers = Model.Modifiers;

            IsActive = Model.IsActive;
        }

        bool _active;

        public bool IsActive
        {
            get => _active;
            set
            {
                _active = value;

                if (value && !IsRegistered)
                {
                    Register();
                }
                else if (!value && IsRegistered)
                {
                    Unregister();
                }

                OnPropertyChanged();
            }
        }

        public bool IsRegistered { get; private set; }

        public ushort Id { get; private set; }

        public void Register()
        {
            if (IsRegistered || Key == Keys.None)
                return;

            // Generate Unique ID
            var uid = Guid.NewGuid().ToString("N");
            Id = Kernel32.GlobalAddAtom(uid);
            
            if (User32.RegisterHotKey(IntPtr.Zero, Id, (int) Modifiers, (uint) Key))
                IsRegistered = true;
            else
            {
                Kernel32.GlobalDeleteAtom(Id);
                Id = 0;
            }
        }
        
        public Keys Key { get; private set; }

        public Modifiers Modifiers { get; private set; }

        public void Change(Keys NewKey, Modifiers NewModifiers)
        {
            Unregister();

            Key = NewKey;
            Modifiers = NewModifiers;

            Register();
        }

        public void Unregister()
        {
            if (!IsRegistered)
                return;

            if (User32.UnregisterHotKey(IntPtr.Zero, Id))
            {
                IsRegistered = false;

                Kernel32.GlobalDeleteAtom(Id);
                Id = 0;
            }
        }

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
            {
                text += Key - Keys.D0;
            }
            else if (Key >= Keys.NumPad0 && Key <= Keys.NumPad9)
            {
                text += Key - Keys.NumPad0;
            }
            else text += Key;

            return text;
        }
    }
}