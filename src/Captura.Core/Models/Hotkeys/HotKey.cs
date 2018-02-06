using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Captura.Models
{
    public class Hotkey : NotifyPropertyChanged
    {
        public TextLocalizer Description { get; }

        public ServiceName ServiceName { get; }

        public Hotkey(HotkeyModel Model)
        {
            ServiceName = Model.ServiceName;
            Key = Model.Key;
            Modifiers = Model.Modifiers;
            Description = new TextLocalizer(GetDescriptionKey(Model.ServiceName));

            IsActive = Model.IsActive;
        }

        static string GetDescriptionKey(ServiceName ServiceName)
        {
            switch (ServiceName)
            {
                case ServiceName.Recording:
                    return nameof(LanguageManager.StartStopRecording);

                case ServiceName.Pause:
                    return nameof(LanguageManager.PauseResumeRecording);

                case ServiceName.ScreenShot:
                    return nameof(LanguageManager.ScreenShot);

                case ServiceName.ActiveScreenShot:
                    return nameof(LanguageManager.ScreenShotActiveWindow);

                case ServiceName.DesktopScreenShot:
                    return nameof(LanguageManager.ScreenShotDesktop);

                default:
                    return SpaceAtCapitals(ServiceName);
            }
        }

        static string SpaceAtCapitals<T>(T Obj)
        {
            var s = Obj.ToString();

            var sb = new StringBuilder();

            for (var i = 0; i < s.Length; ++i)
            {
                if (i != 0 && char.IsUpper(s[i]))
                    sb.Append(" ");

                sb.Append(s[i]);
            }

            return sb.ToString();
        }

        bool _active;

        public bool IsActive
        {
            get => _active;
            set
            {
                _active = value;

                if (value && !IsRegistered)
                    Register();

                else if (!value && IsRegistered)
                    Unregister();

                OnPropertyChanged();
            }
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