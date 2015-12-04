using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Captura
{
    [Flags]
    public enum ModifierKeyCodes : uint
    {
        Alt = 1,
        Control = 2,
        Shift = 4
    }

    public enum KeyCode { R = 0x52, S = 0x53 }

    public class KeyboardHookList : IDisposable
    {
        Random R;

        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, ModifierKeyCodes fdModifiers, KeyCode vk);

        #region Fields
        WindowInteropHelper host;
        IntPtr Handle;
        bool IsDisposed = false;
        Dictionary<int, Action> Keys = new Dictionary<int, Action>();
        #endregion

        ~KeyboardHookList() { Dispose(); }

        public KeyboardHookList(Window Window)
        {
            host = new WindowInteropHelper(Window);
            Handle = host.Handle;

            ComponentDispatcher.ThreadPreprocessMessage += ProcessMessage;

            R = new Random(DateTime.Now.Millisecond);
        }

        public void Register(KeyCode Key, ModifierKeyCodes Modifiers, Action Callback)
        {
            int Identifier = R.Next();

            RegisterHotKey(Handle, Identifier, Modifiers, Key);

            Keys.Add(Identifier, Callback);
        }

        void ProcessMessage(ref MSG msg, ref bool handled)
        {
            if (msg.message == 786)
                foreach (var Key in Keys)
                    if (msg.wParam.ToInt32() == Key.Key)
                        Key.Value();
        }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                foreach (var Identifier in Keys.Keys)
                    UnregisterHotKey(Handle, Identifier);
                host = null;
            }
            IsDisposed = true;
        }
    }
}