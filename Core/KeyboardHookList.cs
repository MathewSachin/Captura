using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Interop;
using System.Runtime.InteropServices;

namespace Captura
{
    [Flags]
    public enum ModifierKeyCodes : uint
    {
        Alt = 1,
        Control = 2,
        Shift = 4
    }

    public enum WindowsMessage { HOTKEY = 786 }

    public enum KeyCode
    {
        A = 0x41,
        B = 0x42,
        C = 0x43,
        D = 0x44,
        E = 0x45,
        F = 0x46,
        G = 0x47,
        H = 0x48,
        I = 0x49,
        J = 0x4A,
        K = 0x4B,
        L = 0x4C,
        M = 0x4D,
        N = 0x4E,
        O = 0x4F,
        P = 0x50,
        Q = 0x51,
        R = 0x52,
        S = 0x53,
        T = 0x54,
        U = 0x55,
        V = 0x56,
        W = 0x57,
        X = 0x58,
        Y = 0x59,
        Z = 0x5A
    }

    public class KeyboardHookList : IDisposable
    {
        Random R;

        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, ModifierKeyCodes fdModifiers, KeyCode vk);

        #region Fields
        WindowInteropHelper host;
        bool IsDisposed = false;
        Dictionary<int, Action> Keys = new Dictionary<int, Action>();
        #endregion

        public KeyboardHookList(Window Window)
        {
            host = new WindowInteropHelper(Window);

            ComponentDispatcher.ThreadPreprocessMessage += ProcessMessage;

            R = new Random(DateTime.Now.Millisecond);
        }

        public void Register(KeyCode Key, ModifierKeyCodes Modifiers, Action Callback)
        {
            int Identifier = R.Next();

            RegisterHotKey(host.Handle, Identifier, Modifiers, Key);

            Keys.Add(Identifier, Callback);
        }

        void ProcessMessage(ref MSG msg, ref bool handled)
        {
            if (msg.message == (int)WindowsMessage.HOTKEY)
                foreach (var Key in Keys)
                    if (msg.wParam.ToInt32() == Key.Key)
                        Key.Value();
        }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                ComponentDispatcher.ThreadPreprocessMessage -= ProcessMessage;

                foreach (var Identifier in Keys.Keys)
                    UnregisterHotKey(host.Handle, Identifier);
                host = null;
            }
            IsDisposed = true;
        }
    }
}